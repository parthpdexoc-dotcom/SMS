using AutoMapper;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing block date operations using the generic service pattern.
    /// </summary>
    public class BlockDateService : GenericService<BlockDateDto, BlockDate>, IBlockDateService
    {
        private readonly IBlockDateRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRequestService _requestService;
        private readonly IPostService _postService;
        private readonly ITokenParser _tokenParser;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDateService"/> class.
        /// </summary>
        /// <param name="repository">The block date repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="tokenParser">The token parser instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="requestService">The request service for foreign key validation.</param>
        /// <param name="postService">The post service for foreign key validation.</param>
        public BlockDateService(
            IBlockDateRepository repository,
            IMapper mapper,
            ITokenParser tokenParser,
            ILogger<BlockDateService> logger,
            IRequestService requestService,
            IPostService postService) : base(repository, mapper, "BlockDateId", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _requestService = requestService;
            _postService = postService;
            _tokenParser = tokenParser;
        }

        /// <summary>
        /// Inserts a new block date or updates an existing one.
        /// </summary>
        /// <param name="dto">The block date DTO to insert or update.</param>
        /// <returns>The upserted block date DTO.</returns>
        public override async Task<BlockDateDto> UpsertAsync(BlockDateDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate foreign keys sequentially (DbContext is not thread-safe; avoid concurrent use)
            if (dto.RequestID.HasValue && dto.RequestID.Value != Guid.Empty)
                await ValidateForeignKeyAsync<IRequestService, RequestDto>(_requestService, dto.RequestID.Value, "Request").ConfigureAwait(false);
            if (dto.PostID.HasValue && dto.PostID.Value != Guid.Empty)
                await ValidateForeignKeyAsync<IPostService, PostDto>(_postService, dto.PostID.Value, "Post").ConfigureAwait(false);

            // If EndDateTime has passed, allow creating a BlockDate with same RequestID and PostID (skip duplicate/overlap checks)
            if (dto.EndDateTime < DateTime.UtcNow)
                return await base.UpsertAsync(dto).ConfigureAwait(false);

            // Validate for duplicate block date (same RequestID, PostID, StartDateTime, EndDateTime)
            if (dto.StartDateTime != default && dto.EndDateTime != default)
            {
                await ValidateCompositeKeyDuplicateAsync(
                    dto: dto,
                    filterExpression: e =>
                        (e.RequestID == dto.RequestID || (e.RequestID == null && dto.RequestID == null)) &&
                        (e.PostID == dto.PostID || (e.PostID == null && dto.PostID == null)) &&
                        e.StartDateTime == dto.StartDateTime &&
                        e.EndDateTime == dto.EndDateTime,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_BLOCK_DATE).ConfigureAwait(false);
            }

            bool isNew = dto.BlockDateId == Guid.Empty;
            bool checkPostOverlap = dto.PostID.HasValue && dto.PostID.Value != Guid.Empty;
            bool checkRequestOverlap = dto.RequestID.HasValue && dto.RequestID.Value != Guid.Empty;

            if (!checkPostOverlap && !checkRequestOverlap)
                return await base.UpsertAsync(dto).ConfigureAwait(false);

            // Run overlap queries sequentially (DbContext is not thread-safe; avoid concurrent use)
            if (checkPostOverlap)
            {
                Expression<Func<BlockDate, bool>> postOverlapFilter = isNew
                    ? (e => e.PostID == dto.PostID && e.StartDateTime <= dto.EndDateTime && e.EndDateTime >= dto.StartDateTime)
                    : (e => e.BlockDateId != dto.BlockDateId && e.PostID == dto.PostID && e.StartDateTime <= dto.EndDateTime && e.EndDateTime >= dto.StartDateTime);
                var existingPostBlockDates = (await GetAllAsync(postOverlapFilter, null, null).ConfigureAwait(false)).ToList();
                if (existingPostBlockDates.Any())
                {
                    var post = await _postService.GetByIdAsync(dto.PostID!.Value).ConfigureAwait(false);
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.OVERLAPPING_BLOCK_DATE_FOR_JOB_ID, !string.IsNullOrWhiteSpace(post?.PostTitle) ? post.PostTitle! : "post"));
                }
            }
            if (checkRequestOverlap)
            {
                Expression<Func<BlockDate, bool>> requestOverlapFilter = isNew
                    ? (e => e.RequestID == dto.RequestID && (e.PostID == dto.PostID || (e.PostID == null && dto.PostID == null)) && e.StartDateTime <= dto.EndDateTime && e.EndDateTime >= dto.StartDateTime)
                    : (e => e.BlockDateId != dto.BlockDateId && e.RequestID == dto.RequestID && (e.PostID == dto.PostID || (e.PostID == null && dto.PostID == null)) && e.StartDateTime <= dto.EndDateTime && e.EndDateTime >= dto.StartDateTime);
                var existingRequestBlockDates = (await GetAllAsync(requestOverlapFilter, null, null).ConfigureAwait(false)).ToList();
                if (existingRequestBlockDates.Any())
                {
                    var request = await _requestService.GetByIdAsync(dto.RequestID!.Value).ConfigureAwait(false);
                    throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.OVERLAPPING_BLOCK_DATE_FOR_REQUEST_ID, !string.IsNullOrWhiteSpace(request?.Title) ? request.Title! : "request"));
                }
            }

            return await base.UpsertAsync(dto).ConfigureAwait(false);
        }

        public override Task<PagedList<BlockDateDto>> SearchAsync(
            string? searchTerm,
            PageParam? pageParam = null,
            Expression<Func<BlockDate, bool>>? filter = null,
            Expression<Func<BlockDate, object>>[]? includes = null,
            params string[] searchableProperties)
        {
            var effectiveIncludes = includes ?? new Expression<Func<BlockDate, object>>[] { x => x.Request, x => x.Post };
            var defaultFilter = filter ?? (x => true);

            return SearchPagedAsync(
                searchTerm,
                defaultFilter,
                term =>
                {
                    var lowered = term.Trim().ToLower();
                    return x =>
                        (x.Request != null && x.Request.Title != null && x.Request.Title.ToLower().Contains(lowered))
                        || (x.Post != null && x.Post.PostTitle != null && x.Post.PostTitle.ToLower().Contains(lowered));
                },
                pageParam,
                effectiveIncludes);
        }
    }
}
