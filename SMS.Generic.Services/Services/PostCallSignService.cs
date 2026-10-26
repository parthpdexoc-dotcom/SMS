using AutoMapper;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class PostCallSignService : GenericService<PostCallSignDto, PostCallSign>, IPostCallSignService
    {
        private static readonly PostPositionStatus[] OccupyingStatuses =
        {
            PostPositionStatus.Assigned,
            PostPositionStatus.SplitDecisionPending,
            PostPositionStatus.CancellationPending,
            PostPositionStatus.Unclosed,
            PostPositionStatus.Closed
        };

        private readonly IPostPositionRepository _postPositionRepository;

        public PostCallSignService(
            IPostCallSignRepository repository,
            IPostPositionRepository postPositionRepository,
            IMapper mapper,
            ITokenParser tokenParser) : base(repository, mapper, "PostCallSignID", tokenParser)
        {
            _postPositionRepository = postPositionRepository ?? throw new ArgumentNullException(nameof(postPositionRepository));
        }

        public async Task<IEnumerable<PostCallSignDto>> GetAvailableForDropdownAsync(Guid postId, DateOnly dateOfWork)
        {
            var usedCallSignIds = (await _postPositionRepository.GetAllAsync(
                    filter: x =>
                        x.PostID == postId &&
                        x.DateOfWork == dateOfWork &&
                        OccupyingStatuses.Contains(x.PostPositionStatus) &&
                        x.PostCallSignID.HasValue &&
                        x.PostCallSignID.Value != Guid.Empty,
                    includes: null,
                    pageParam: null).ConfigureAwait(false))
                .Select(x => x.PostCallSignID!.Value)
                .ToHashSet();

            return (await GetAllAsync(
                    filter: x => x.PostID == postId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false))
                .Where(x => x.PostCallSignID.HasValue
                    && x.PostCallSignID.Value != Guid.Empty
                    && !usedCallSignIds.Contains(x.PostCallSignID.Value))
                .OrderBy(x => x.CreatedDate ?? DateTime.MaxValue)
                .ThenBy(x => x.PostCallSignID);
        }

        public async Task<PostCallSignDto?> GetNextAvailableAsync(Guid postId, DateOnly dateOfWork) =>
            (await GetAvailableForDropdownAsync(postId, dateOfWork).ConfigureAwait(false)).FirstOrDefault();

        public async Task SoftDeleteByPostIdAsync(Guid postId)
        {
            await UpdateAsync(
                    x => x.PostID == postId,
                    x => PopulateSoftDeleteFields(x))
                .ConfigureAwait(false);
        }

        public async Task CloneForPostsAsync(IReadOnlyDictionary<Guid, Guid> oldToNewPostIdMap)
        {
            if (oldToNewPostIdMap == null || oldToNewPostIdMap.Count == 0)
                return;

            var sourcePostIds = oldToNewPostIdMap.Keys.ToHashSet();
            var sourceCallSigns = await GetAllAsync(
                    x => sourcePostIds.Contains(x.PostID),
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false);

            var cloned = sourceCallSigns
                .Where(x => x.PostCallSignID.HasValue
                    && x.PostCallSignID.Value != Guid.Empty
                    && !string.IsNullOrWhiteSpace(x.CallSign))
                .Select(x => x.CloneForPost(oldToNewPostIdMap[x.PostID]))
                .ToList();

            if (cloned.Count > 0)
                await base.BulkUpsertAsync(cloned).ConfigureAwait(false);
        }

        public override async Task<IEnumerable<PostCallSignDto>> BulkUpsertAsync(
            IEnumerable<PostCallSignDto> dtos,
            bool isSuperUser = false)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtoList = dtos.Where(x => x != null).ToList();

            var idsToDelete = dtoList
                .Where(x => string.IsNullOrWhiteSpace(x.CallSign)
                    && x.PostCallSignID.HasValue
                    && x.PostCallSignID.Value != Guid.Empty)
                .Select(x => x.PostCallSignID!.Value)
                .Distinct()
                .ToList();

            if (idsToDelete.Count > 0)
            {
                await UpdateAsync(
                        x => idsToDelete.Contains(x.PostCallSignID),
                        x => PopulateSoftDeleteFields(x))
                    .ConfigureAwait(false);
            }

            var itemsWithCallSign = dtoList
                .Where(x => !string.IsNullOrWhiteSpace(x.CallSign))
                .ToList();

            if (itemsWithCallSign.Count == 0)
                return Array.Empty<PostCallSignDto>();

            // Skip create-only rows when the post already has a catalog (avoids duplicate inserts).
            // Rows with a real ID, or new slots in a mixed update batch, are still allowed.
            var postIds = itemsWithCallSign.Select(i => i.PostID).Distinct().ToList();
            var existingPostIds = (await GetAllAsync(
                    x => postIds.Contains(x.PostID),
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false))
                .Select(x => x.PostID)
                .ToHashSet();

            var postsWithUpdates = itemsWithCallSign
                .Where(x => x.PostCallSignID.HasValue && x.PostCallSignID.Value != Guid.Empty)
                .Select(x => x.PostID)
                .ToHashSet();

            itemsWithCallSign = itemsWithCallSign
                .Where(x => (x.PostCallSignID.HasValue && x.PostCallSignID.Value != Guid.Empty)
                    || !existingPostIds.Contains(x.PostID)
                    || postsWithUpdates.Contains(x.PostID))
                .ToList();

            if (itemsWithCallSign.Count == 0)
                return Array.Empty<PostCallSignDto>();

            return await base.BulkUpsertAsync(itemsWithCallSign, isSuperUser).ConfigureAwait(false);
        }
    }
}
