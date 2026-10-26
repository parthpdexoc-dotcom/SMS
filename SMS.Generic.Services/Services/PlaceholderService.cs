using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services
{
    public class PlaceholderService : GenericService<PlaceholderDto, Placeholder>, IPlaceholderService
    {
        private readonly IPostPositionRepository _postPositionRepository;
        private readonly IMapper _mapper;
        private readonly ITokenParser _tokenParser;

        public PlaceholderService(
            IPlaceholderRepository repository,
            IPostPositionRepository postPositionRepository,
            IMapper mapper,
            ILogger<PlaceholderService> logger,
            ITokenParser tokenParser)
            : base(repository, mapper, nameof(Placeholder.PlaceholderID), tokenParser)
        {
            _postPositionRepository = postPositionRepository ?? throw new ArgumentNullException(nameof(postPositionRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tokenParser = tokenParser ?? throw new ArgumentNullException(nameof(tokenParser));
        }

        public async Task<PagedList<PlaceholderDto>> GetAllAsync(PlaceholderStatus? status = null, PageParam? pageParam = null)
        {
            pageParam ??= new PageParam();
            if (string.IsNullOrWhiteSpace(pageParam.SortBy))
                pageParam.SortBy = nameof(PlaceholderDto.LastName);

            return await GetAllPagedAsync(BuildStatusFilter(status), pageParam).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<PlaceholderDto>> SearchAsync(string? searchTerm, PlaceholderStatus? status = null)
        {
            var items = (await base.GetAllAsync(BuildStatusFilter(status), null, null).ConfigureAwait(false))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList();
            if (string.IsNullOrWhiteSpace(searchTerm))
                return items;

            var tokens = searchTerm.ToLower().Replace(",", " ").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return items.Where(x =>
                tokens.All(t => new[] { x.FirstName, x.LastName, x.EmployeeNumber }
                    .Any(v => v?.ToLower().Replace(",", "").Contains(t) == true))).ToList();
        }

        public async Task ValidateBulkEmployeeNumbersAsync(IEnumerable<BulkAssignPlaceholderItemDto>? placeholders)
        {
            var numbers = (placeholders ?? [])
                .Where(x => (!x.PlaceholderID.HasValue || x.PlaceholderID == Guid.Empty)
                    && !x.IsUniquePerPosition
                    && !string.IsNullOrWhiteSpace(x.EmployeeNumber))
                .Select(x => x.EmployeeNumber!.Trim())
                .ToList();

            if (numbers.Count == 0)
                return;

            var hasDuplicate = numbers.Count != numbers.Distinct(StringComparer.OrdinalIgnoreCase).Count();

            if (!hasDuplicate)
            {
                var departmentId = _tokenParser.GetDepartmentID()
                    ?? throw new UnauthorizedAccessException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.GENERIC_DEPARTMENT_ID_MISSING_FROM_TOKEN));

                hasDuplicate = await GetFilteredCountAsync(
                    x => x.DepartmentID == departmentId
                        && x.EmployeeNumber != null
                        && numbers.Contains(x.EmployeeNumber)).ConfigureAwait(false) > 0;
            }

            if (hasDuplicate)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_PLACEHOLDER_EMPLOYEE_NUMBER));
            }
        }

        public async Task CreateMissingAndSetIdsAsync(IEnumerable<BulkAssignPlaceholderItemDto>? placeholders)
        {
            foreach (var item in placeholders ?? [])
            {
                var hasPlaceholderId = item.PlaceholderID.HasValue && item.PlaceholderID.Value != Guid.Empty;
                var hasName = !string.IsNullOrWhiteSpace(item.FirstName) && !string.IsNullOrWhiteSpace(item.LastName);
                if (hasPlaceholderId || !hasName)
                    continue;

                if (item.IsUniquePerPosition)
                    item.EmployeeNumber = null;

                var saved = await UpsertAsync(_mapper.Map<PlaceholderDto>(item)).ConfigureAwait(false);
                item.PlaceholderID = saved.PlaceholderID;
            }
        }

        public async Task<SuccessResult<List<BulkPlaceholderStatusItemDto>>> BulkUpdateStatusAsync(IReadOnlyDictionary<Guid, PlaceholderStatus> placeholders)
        {
            var result = new SuccessResult<List<BulkPlaceholderStatusItemDto>>
            {
                Status = (int)HttpStatusCode.OK,
                Data = new List<BulkPlaceholderStatusItemDto>()
            };

            var statuses = (placeholders ?? new Dictionary<Guid, PlaceholderStatus>())
                .Where(kv => kv.Key != Guid.Empty)
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.Last().Value);

            if (statuses.Count == 0)
                return result;

            var ids = statuses.Keys.ToList();
            // GetAllAsync already applies department scope from the token (GenericService).
            var existing = (await base.GetAllAsync(x => ids.Contains(x.PlaceholderID), null, null).ConfigureAwait(false))
                .Where(x => x.PlaceholderID.HasValue)
                .ToList();

            if (existing.Count == 0)
                return result;

            var toUpdate = existing
                .Where(x => x.PlaceholderStatus != statuses[x.PlaceholderID!.Value])
                .ToList();
            if (toUpdate.Count == 0)
                return result;

            var archiveIds = toUpdate
                .Where(x => statuses[x.PlaceholderID!.Value] == PlaceholderStatus.Archived)
                .Select(x => x.PlaceholderID!.Value)
                .ToList();
            var blockedIds = archiveIds.Count == 0
                ? new HashSet<Guid>()
                : (await _postPositionRepository.GetAllAsync(
                    x => x.PlaceholderID.HasValue
                        && archiveIds.Contains(x.PlaceholderID.Value)
                        && x.PostPositionStatus != PostPositionStatus.Cancelled,
                    null,
                    null).ConfigureAwait(false))
                    .Select(x => x.PlaceholderID!.Value)
                    .ToHashSet();

            var successMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PLACEHOLDER_STATUS_UPDATED);
            var assignmentMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PLACEHOLDER_HAS_ASSIGNMENT_HISTORY);
            var unarchiveMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.PLACEHOLDER_CANNOT_BE_UNARCHIVED);
            var updateIds = new List<Guid>();
            foreach (var item in toUpdate)
            {
                var id = item.PlaceholderID!.Value;
                if (item.PlaceholderStatus == PlaceholderStatus.Archived)
                {
                    result.Data.Add(new BulkPlaceholderStatusItemDto
                    {
                        PlaceholderID = id,
                        PlaceholderName = item.GetPlaceHolderName(),
                        Message = unarchiveMessage,
                        HasError = true
                    });
                    continue;
                }

                if (blockedIds.Contains(id))
                {
                    result.Data.Add(new BulkPlaceholderStatusItemDto
                    {
                        PlaceholderID = id,
                        PlaceholderName = item.GetPlaceHolderName(),
                        Message = assignmentMessage,
                        HasError = true
                    });
                    continue;
                }

                updateIds.Add(id);
                result.Data.Add(new BulkPlaceholderStatusItemDto
                {
                    PlaceholderID = id,
                    PlaceholderName = item.GetPlaceHolderName(),
                    Message = successMessage,
                    HasError = false
                });
            }

            if (updateIds.Count > 0)
            {
                await UpdateAsync(
                    x => updateIds.Contains(x.PlaceholderID),
                    x => x.PlaceholderStatus = statuses[x.PlaceholderID]).ConfigureAwait(false);
            }

            return result;
        }

        private static Expression<Func<Placeholder, bool>> BuildStatusFilter(PlaceholderStatus? status)
        {
            if (!status.HasValue)
                return x => x.PlaceholderStatus == PlaceholderStatus.Visible
                    || x.PlaceholderStatus == PlaceholderStatus.Hidden;

            var requested = status.Value;
            return x => x.PlaceholderStatus == requested;
        }
    }
}
