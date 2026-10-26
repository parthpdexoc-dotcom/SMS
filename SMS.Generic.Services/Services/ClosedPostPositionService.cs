using AutoMapper;
using SMS.Generic.Common;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing ClosedPostPosition operations using the generic service pattern.
    /// </summary>
    public class ClosedPostPositionService : GenericService<ClosedPostPositionDto, ClosedPostPosition>, IClosedPostPositionService
    {
        private readonly IClosedPostPositionRepository _closedPostPositionRepository;
        private readonly ITokenParser? _tokenParser;
        private readonly IPostPositionService _postPositionService;
        private readonly IPostService _postService;
        private readonly IResponseService _responseService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionService"/> class.
        /// </summary>
        /// <param name="repository">The closed post position repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="postPositionService">The post position service for FK validation.</param>
        /// <param name="employeeService">The employee service for FK validation.</param>
        /// <param name="postService">The post service for FK validation.</param>
        /// <param name="responseService">The response service for FK validation (questionnaire response).</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public ClosedPostPositionService(
            IClosedPostPositionRepository repository,
            IMapper mapper,
            IPostPositionService postPositionService,
            IPostService postService,
            IResponseService responseService,
            IEmployeeService employeeService,
            IEmailTemplateService emailTemplateService,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "ClosedPostPositionID", tokenParser)
        {
            _tokenParser = tokenParser;
            _postPositionService = postPositionService;
            _postService = postService;
            _responseService = responseService;
            _closedPostPositionRepository = repository;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<PagedList<EmployeePostListingItemDto>> GetLatestPerPostPositionPagedAsync(PageParam? pagination = null, Guid? employeeId = null)
        {
            var p = pagination ?? new PageParam();
            
            employeeId ??= _tokenParser?.GetEmployeeID();
        
            var latestEntities = await _closedPostPositionRepository
                .GetLatestPerPostPositionForEmployeeAsync(employeeId.Value)
                .ConfigureAwait(false);
            var latest = _mapper.Map<List<ClosedPostPositionDto>>(latestEntities);

            var listing = new List<EmployeePostListingItemDto>(latest.Count);
            for (var i = 0; i < latest.Count; i++)
                listing.Add(latest[i].ToEmployeePostListingItem());

            return new PagedList<EmployeePostListingItemDto>(
                listing.Skip(p.Offset ?? 0).Take(p.GetLimit() ?? PageParam.MinLimit).ToList(),
                p,
                listing.Count);
        }

        /// <inheritdoc />
        public async Task<PagedList<CloseAssignmentClosedPostPositionResponseDto>> GetCloseAssignmentsForLoggedInEmployeeAsync(
            string? searchTerm = null,
            PageParam? pagination = null,
            Guid? employeeId = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            bool isPlaceholderOnly = false,
            Guid? requesterId = null)
        {
            var effectivePageParam = pagination ?? new PageParam();

            var statusFilter = new[]
            {
                ClosedPostPositionStatus.Closed,
                ClosedPostPositionStatus.Unclosed
            };

            var hasExplicitEmployeeId = employeeId.HasValue && employeeId.Value != Guid.Empty;
            var shouldFilterByEmployee = hasExplicitEmployeeId && !isPlaceholderOnly;
            var effectiveEmployeeId = hasExplicitEmployeeId ? employeeId : null;

            var effectiveDepartmentId = _tokenParser.GetDepartmentID();
            var hasDepartmentFilter = effectiveDepartmentId.HasValue && effectiveDepartmentId.Value != Guid.Empty;

            var (items, total) = await _closedPostPositionRepository
                .GetCloseAssignmentsPageAsync(
                    statusFilter,
                    searchTerm,
                    shouldFilterByEmployee,
                    effectiveEmployeeId,
                    hasDepartmentFilter,
                    hasDepartmentFilter ? effectiveDepartmentId : null,
                    startDate,
                    endDate,
                    effectivePageParam,
                    isPlaceholderOnly,
                    requesterId)
                .ConfigureAwait(false);

            var dtos = _mapper.Map<List<CloseAssignmentClosedPostPositionResponseDto>>(items);
            return new PagedList<CloseAssignmentClosedPostPositionResponseDto>(dtos, effectivePageParam, total);
        }

        /// <summary>
        /// Updates closed post position status and optionally IsEdited.
        /// </summary>
        public async Task<int> UpdateClosedPostPositionStatusAsync(Guid closedPostPositionId, ClosedPostPositionStatus status, bool? isEdited = null)
        {
            if (closedPostPositionId == Guid.Empty)
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_ID)
                    ?? ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_ID,
                    nameof(closedPostPositionId));

            var existing = await GetByIdAsync(closedPostPositionId);
            if (existing == null)
                return 0;

            if (existing.Status == status && (!isEdited.HasValue || existing.IsEdited == isEdited.Value))
                return 1;

            return await UpdateAsync(
                filter: x => x.ClosedPostPositionID == closedPostPositionId,
                updateAction: entity =>
                {
                    entity.Status = status;
                    if (isEdited.HasValue)
                        entity.IsEdited = isEdited.Value;
                });
        }

        /// <summary>
        /// Inserts a new closed post position record or updates an existing one.
        /// </summary>
        /// <param name="dto">The closed post position DTO to insert or update.</param>
        /// <returns>The upserted closed post position DTO.</returns>
        public override async Task<ClosedPostPositionDto> UpsertAsync(ClosedPostPositionDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // When both set: validate PostPosition exists and belongs to Post (validates both FKs in one go)
            if (dto.PostPositionID.HasValue && dto.PostID.HasValue)
            {
                if (dto.PostPositionID.HasValue)
                    await ValidateForeignKeyAsync<IPostPositionService, PostPositionDto>(
                        _postPositionService, dto.PostPositionID.Value, "PostPosition");
                if (dto.PostID.HasValue)
                    await ValidateForeignKeyAsync<IPostService, PostDto>(
                        _postService, dto.PostID.Value, "Post");
                var postPosition = await _postPositionService.GetByIdAsync(dto.PostPositionID.Value);
                if (postPosition == null || postPosition.PostID != dto.PostID.Value)
                {
                    var message = ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_JOB_AND_JOB_POSITION)
                        ?? ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_JOB_AND_JOB_POSITION;
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        message);
                }
            }

            if (dto.QuestionaireResponseID.HasValue)
            {
                await ValidateForeignKeyAsync<IResponseService, ResponseDto>(
                    _responseService, dto.QuestionaireResponseID.Value, "QuestionaireResponse");
            }

            dto.ApplyMinimumHoursPaidPolicy(_tokenParser?.GetDepartmentPolicy());

            return await base.UpsertAsync(dto);
        }
    }
}
