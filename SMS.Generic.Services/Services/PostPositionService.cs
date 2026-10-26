using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMS.Generic.Common;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Helpers;
using SMS.Generic.Services.Interfaces;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing post operations using the generic service pattern.
    /// </summary>
    public class PostPositionService : GenericService<PostPositionDto, PostPosition>, IPostPositionService
    {
        private static readonly IReadOnlyDictionary<string, PropertyInfo> _updateCallSignAllowedProperties =
            new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(PostPosition.CallSign), GetWritableStringProperty(nameof(PostPosition.CallSign)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue1), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue1)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue2), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue2)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue3), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue3)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue4), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue4)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue5), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue5)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue6), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue6)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue7), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue7)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue8), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue8)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue9), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue9)) },
                { nameof(PostPosition.PostDepartmentDefinedFieldValue10), GetWritableStringProperty(nameof(PostPosition.PostDepartmentDefinedFieldValue10)) }
            };

        /// <summary>
        /// Statuses that count as a taken slot (available-post logic). Use <c>.Contains</c> in LINQ-to-EF filters — not a custom method EF cannot translate.
        /// </summary>
        private static readonly PostPositionStatus[] TakenSlotOccupyingStatuses =
        {
            PostPositionStatus.Assigned,
            PostPositionStatus.SplitDecisionPending,
            PostPositionStatus.CancellationPending,
            PostPositionStatus.Unclosed,
            PostPositionStatus.Closed
        };

        private readonly IPostPositionRepository _repository;
        private readonly IRequestService _requestService;
        private readonly IRequesterService _requesterService;
        private readonly IPostService _postService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeTypeService _employeeTypeService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IClosedPostPositionRepository _closedPostPositionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITokenParser _tokenParser;
        private readonly IFirebasePushNotificationService _pushNotificationService;
        private readonly IPostWorkflowAuditService _postWorkflowAuditService;
        private readonly IPostPositionFlowAuditService _postPositionFlowAuditService;
        private readonly IWaitingPostPositionService _waitingPostPositionService;
        private readonly IDepartmentConfigurationRepository _departmentConfigurationRepository;
        private readonly IPlaceholderService _placeholderService;
        private readonly IPostCallSignService _postCallSignService;

        private static readonly Expression<Func<PostPosition, object>>[] DefaultDetailIncludes =
        {
            x => x.Post,
            x => x.Post.Request,
            x => x.Post.Request.Requester,
            x => x.Post.PayCode,
            x => x.Post.EmployeeType,
            x => x.Employee,
            x => x.Placeholder,
            x => x.PostCallSign!
        };

        private readonly IViewDetailsEquipmentRepository _viewDetailsEquipmentRepository;
        private readonly IPostPositionVehicleRepository _postPositionVehicleRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="PostService"/> class.
        /// </summary>
        /// <param name="repository">The post repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        public PostPositionService(
            IPostPositionRepository repository,
            IMapper mapper,
            ILogger<PostPositionService> logger,
            ITokenParser tokenParser,
            IRequestService requestService,
            IRequesterService requesterService,
            IPostService postService,
            IEmployeeRepository employeeRepository,
            IEmployeeTypeService employeeTypeService,
            IEmailTemplateService emailTemplateService,
            IClosedPostPositionRepository closedPostPositionRepository,
            IFirebasePushNotificationService pushNotificationService,
            IPostWorkflowAuditService postWorkflowAuditService,
            IPostPositionFlowAuditService postPositionFlowAuditService,
            IWaitingPostPositionService waitingPostPositionService,
            IPlaceholderService placeholderService,
            IDepartmentConfigurationRepository departmentConfigurationRepository,
            IViewDetailsEquipmentRepository viewDetailsEquipmentRepository,
            IPostPositionVehicleRepository postPositionVehicleRepository,
            IPostCallSignService postCallSignService) : base(repository, mapper, "PostPositionID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _tokenParser = tokenParser;
            _requestService = requestService;
            _requesterService = requesterService;
            _postService = postService;
            _employeeRepository = employeeRepository;
            _employeeTypeService = employeeTypeService;
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _closedPostPositionRepository = closedPostPositionRepository ?? throw new ArgumentNullException(nameof(closedPostPositionRepository));
            _pushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
            _postWorkflowAuditService = postWorkflowAuditService ?? throw new ArgumentNullException(nameof(postWorkflowAuditService));
            _postPositionFlowAuditService = postPositionFlowAuditService ?? throw new ArgumentNullException(nameof(postPositionFlowAuditService));
            _waitingPostPositionService = waitingPostPositionService ?? throw new ArgumentNullException(nameof(waitingPostPositionService));
            _departmentConfigurationRepository = departmentConfigurationRepository ?? throw new ArgumentNullException(nameof(departmentConfigurationRepository));
            _viewDetailsEquipmentRepository = viewDetailsEquipmentRepository
                ?? throw new ArgumentNullException(nameof(viewDetailsEquipmentRepository));
            _postPositionVehicleRepository = postPositionVehicleRepository
                ?? throw new ArgumentNullException(nameof(postPositionVehicleRepository));
            _placeholderService = placeholderService ?? throw new ArgumentNullException(nameof(placeholderService));
            _postCallSignService = postCallSignService ?? throw new ArgumentNullException(nameof(postCallSignService));
        }

        public override async Task<PostPositionDto> UpsertAsync(PostPositionDto dto, bool isSuperUser = false)
        {
            try
            {
                ValidateDtoParameter(dto);
                ValidatePlaceholderRules(dto);
                await ValidateDuplicatePlaceholderAssignmentAsync(dto).ConfigureAwait(false);
                await UpsertPlaceholderAndAssignIdAsync(dto).ConfigureAwait(false);
                if (dto.PostPositionStatus == PostPositionStatus.Assigned && !dto.IsPlaceholder)
                {
                    var existingPostPositions = await GetAllAsync(x => x.PostPositionID != dto.PostPositionID &&   // ignore self update
                    x.PostID == dto.PostID &&
                    x.EmployeeID == dto.EmployeeID &&
                    x.DateOfWork == dto.DateOfWork &&
                    x.PostPositionStatus == PostPositionStatus.Assigned &&
                    x.StartTime == dto.PostStartTime &&
                    x.EndTime == dto.PostEndTime);

                    bool alreadyExists = existingPostPositions.Any();

                    if (alreadyExists)
                    {
                        throw new WebApiException(HttpStatusCode.BadRequest, "400", ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_JOB_POSITION_ASSIGNMENT));
                    }
                }

                var result = await base.UpsertAsync(dto);
                await _postWorkflowAuditService.LogSuccessAsync(
                    dto.PostPositionStatus,
                    PostWorkflowAuditContext.FromResult(result, postId: result.PostID));

                return result;
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    dto.PostPositionStatus,
                    ex,
                    new PostWorkflowAuditContext
                    {
                        PostPositionId = dto.PostPositionID,
                        PostId = dto.PostID,
                        EmployeeId = dto.EmployeeID,
                        FailureReason = ex.GetUserFacingMessage()
                    });
                throw;
            }
        }

        private static void ValidatePlaceholderRules(PostPositionDto dto)
        {
            if (dto.IsPlaceholder)
            {
                if (dto.EmployeeID != Guid.Empty)
                    throw new WebApiException(HttpStatusCode.BadRequest, "400", "When isPlaceholder is true, employeeId must be empty.");

                var hasExistingId = dto.PlaceholderID.HasValue && dto.PlaceholderID.Value != Guid.Empty
                    || (dto.PlaceholderDetail?.PlaceholderID is Guid detailId && detailId != Guid.Empty);
                var hasDetail = dto.PlaceholderDetail != null
                    && !string.IsNullOrWhiteSpace(dto.PlaceholderDetail.FirstName)
                    && !string.IsNullOrWhiteSpace(dto.PlaceholderDetail.LastName);

                if (!hasExistingId && !hasDetail)
                    throw new WebApiException(HttpStatusCode.BadRequest, "400", "Placeholder first name and last name are required when isPlaceholder is true.");

                dto.EmployeeID = Guid.Empty;
            }
            else
            {
                if (dto.EmployeeID == Guid.Empty)
                    throw new WebApiException(HttpStatusCode.BadRequest, "400", "EmployeeId is required when isPlaceholder is false.");

                dto.PlaceholderID = null;
                dto.PlaceholderDetail = null;
            }
        }

        /// <summary>
        /// Same rule as an employee: one placeholder cannot be assigned twice to the same job, date, and shift.
        /// Uses the placeholder being assigned, before a unique-per-position copy is created.
        /// </summary>
        private async Task ValidateDuplicatePlaceholderAssignmentAsync(PostPositionDto dto)
        {
            if (!dto.IsPlaceholder || dto.PostPositionStatus != PostPositionStatus.Assigned)
                return;

            var placeholderId = dto.PlaceholderDetail?.PlaceholderID is Guid detailId && detailId != Guid.Empty
                ? detailId
                : dto.PlaceholderID is Guid id && id != Guid.Empty ? id : Guid.Empty;
            if (placeholderId == Guid.Empty)
                return;

            var currentPostPositionId = dto.PostPositionID.GetValueOrDefault();
            var alreadyAssigned = await GetAllAsync(x =>
                x.PostPositionID != currentPostPositionId &&
                x.PostID == dto.PostID &&
                x.PlaceholderID == placeholderId &&
                x.DateOfWork == dto.DateOfWork &&
                x.PostPositionStatus == PostPositionStatus.Assigned &&
                x.StartTime == dto.PostStartTime &&
                x.EndTime == dto.PostEndTime).ConfigureAwait(false);

            if (alreadyAssigned.Any())
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_PLACEHOLDER_JOB_POSITION_ASSIGNMENT));
            }
        }

        /// <summary>
        /// When the client sends placeholder profile data: insert/update <c>Placeholder</c>, then set <see cref="PostPositionDto.PlaceholderID"/> on this row.
        /// <see cref="PostPositionDto.PlaceholderDetail"/> is cleared afterward so only the FK is mapped to <c>PostPosition</c>.
        /// Existing id-only selections are linked without creating a new profile.
        /// </summary>
        private async Task UpsertPlaceholderAndAssignIdAsync(PostPositionDto dto)
        {
            if (!dto.IsPlaceholder)
                return;

            var detail = dto.PlaceholderDetail;

            // Existing placeholder selected by id only — link without creating/updating the profile.
            var detailId = detail?.PlaceholderID;
            var hasDetailId = detailId.HasValue && detailId.Value != Guid.Empty;
            var hasName = detail != null
                && !string.IsNullOrWhiteSpace(detail.FirstName)
                && !string.IsNullOrWhiteSpace(detail.LastName);

            var sourceId = hasDetailId
                ? detailId!.Value
                : dto.PlaceholderID is Guid placeholderId && placeholderId != Guid.Empty ? placeholderId : Guid.Empty;

            if (sourceId != Guid.Empty)
            {
                var existing = await _placeholderService.GetByIdAsync(sourceId).ConfigureAwait(false);
                if (existing?.IsUniquePerPosition == true)
                {
                    if (detail != null)
                        detail.IsUniquePerPosition = true;

                    var currentPostPositionId = dto.PostPositionID.GetValueOrDefault();
                    var assignedToAnotherPost = await GetFilteredCountAsync(pp =>
                        pp.PlaceholderID == sourceId && pp.PostPositionID != currentPostPositionId).ConfigureAwait(false) > 0;

                    if (assignedToAnotherPost)
                    {
                        var created = await _placeholderService.UpsertAsync(new PlaceholderDto
                        {
                            FirstName = existing.FirstName,
                            LastName = existing.LastName,
                            ContactNumber = existing.ContactNumber,
                            DepartmentID = existing.DepartmentID,
                            PlaceholderStatus = existing.PlaceholderStatus,
                            IsUniquePerPosition = false
                        }).ConfigureAwait(false);

                        dto.PlaceholderID = created.PlaceholderID;
                        dto.PlaceholderDetail = null;
                        return;
                    }
                }
            }

            if (hasDetailId && !hasName)
            {
                dto.PlaceholderID = detailId;
                dto.PlaceholderDetail = null;
                return;
            }

            if (hasName)
            {
                if (!hasDetailId)
                    detail!.PlaceholderID = dto.PlaceholderID;

                var saved = await _placeholderService.UpsertAsync(detail).ConfigureAwait(false);
                dto.PlaceholderID = saved.PlaceholderID;
                dto.PlaceholderDetail = null;
                return;
            }

            if (!dto.PlaceholderID.HasValue || dto.PlaceholderID.Value == Guid.Empty)
                throw new WebApiException(HttpStatusCode.BadRequest, "400", "Placeholder first name and last name are required when isPlaceholder is true.");
        }

        public async Task<int> CopyTenureWaitingToPostPositionsAsync(
            IReadOnlyList<WaitingPostPositionDto> tenureRows,
            CancellationToken cancellationToken = default)
        {
            if (tenureRows == null || tenureRows.Count == 0)
                return 0;

            var distinct = tenureRows
                .Where(r => r.PostID is { } pid && pid != Guid.Empty && r.EmployeeID != Guid.Empty && r.DateOfWork != null)
                .GroupBy(r => new { r.PostID, r.EmployeeID, DateOfWork = r.DateOfWork! })
                .Select(g => g
                    .OrderBy(x => x.OrderNumber)
                    .ThenBy(x => x.CreatedDate ?? DateTime.MaxValue)
                    .ThenBy(x => x.WaitingPostPositionID)
                    .First())
                .ToList();

            if (distinct.Count == 0)
                return 0;

            var exemptDepartments = new HashSet<Guid>();
            foreach (var departmentId in distinct
                .Where(r => r.Post?.IsExemptHours == true && r.Post.Request != null && r.Post.Request.DepartmentID != Guid.Empty)
                .Select(r => r.Post!.Request!.DepartmentID)
                .Distinct())
            {
                if (await IsDepartmentExemptFromHoursRulesAsync(departmentId).ConfigureAwait(false))
                    exemptDepartments.Add(departmentId);
            }

            var entities = distinct.Select(r =>
            {
                if (!r.PostID.HasValue || r.PostID.Value == Guid.Empty)
                    throw new InvalidOperationException("Tenure row must have a non-empty PostID.");

                var postId = r.PostID.Value;
                var endTime = r.Post != null
                    ? r.Post.PostEndTime
                    : r.StartTime.AddMinutes(r.LengthOfWork);

                return new PostPosition
                {
                    PostPositionID = Guid.Empty, // DB generates

                    PostID = postId,
                    EmployeeID = r.EmployeeID,
                    DateOfWork = r.DateOfWork,
                    StartTime = r.StartTime,
                    EndTime = endTime,
                    LengthOfWork = r.LengthOfWork,
                    EquipmentRequired = r.EquipmentRequired,
                    CallSign = r.CallSign,
                    PostCallSignID = r.PostCallSignID,
                    PostPositionPayRate = r.WaitingPostPositionPayRate,
                    IsPremiumPaidEvent = r.IsPremiumPaidEvent,
                    IsExemptPosition = r.Post?.IsExemptHours == true
                        && r.Post.Request != null
                        && exemptDepartments.Contains(r.Post.Request.DepartmentID),
                    PostPositionStatus = PostPositionStatus.Assigned,

                    ParentPostPositionId = r.ParentPostPositionId,
                    IsSplit = r.IsSplit,
                    SplitPostPositionID = r.SplitPostPositionID,

                    PostDepartmentDefinedFieldValue1 = r.PostDepartmentDefinedFieldValue1,
                    PostDepartmentDefinedFieldValue2 = r.PostDepartmentDefinedFieldValue2,
                    PostDepartmentDefinedFieldValue3 = r.PostDepartmentDefinedFieldValue3,
                    PostDepartmentDefinedFieldValue4 = r.PostDepartmentDefinedFieldValue4,
                    PostDepartmentDefinedFieldValue5 = r.PostDepartmentDefinedFieldValue5,
                    PostDepartmentDefinedFieldValue6 = r.PostDepartmentDefinedFieldValue6,
                    PostDepartmentDefinedFieldValue7 = r.PostDepartmentDefinedFieldValue7,
                    PostDepartmentDefinedFieldValue8 = r.PostDepartmentDefinedFieldValue8,
                    PostDepartmentDefinedFieldValue9 = r.PostDepartmentDefinedFieldValue9,
                    PostDepartmentDefinedFieldValue10 = r.PostDepartmentDefinedFieldValue10,

                    CreatedDate = r.CreatedDate,
                    CreatedUserID = r.CreatedUserID,
                    CreatedBy = r.CreatedBy,
                    EditedDate = r.EditedDate,
                    EditedUserID = r.EditedUserID,
                    EditedBy = r.EditedBy
                };
            }).ToList();

            await _repository.AddRangeAsync(entities).ConfigureAwait(false);
            return entities.Count;
        }
        public async Task<List<PostPositionDto>> GetPostPositionByEmployeeAndDateRangeAsync(Guid employeeId,DateOnly startDate,DateOnly endDate,Guid? departmentId = null,bool excludeCancelledOnly = true,bool enableExemptRequestsJobsFromHoursRules = false)
        {
            var postpositions = await _repository.GetPostPositionByEmployeeAndDateRangeAsync(employeeId,startDate,endDate,departmentId,excludeCancelledOnly,enableExemptRequestsJobsFromHoursRules).ConfigureAwait(false);
            return _mapper.Map<List<PostPositionDto>>(postpositions);
        }

        public async Task<PostPositionDto> ValidateTransferSourceAsync(
            Guid sourcePostPositionId,
            Guid requestedPostId,
            DateOnly requestedDateOfWork,
            Guid targetEmployeeId)
        {
            var source = await GetByIdAsync(sourcePostPositionId).ConfigureAwait(false);
            if (source == null || !source.PostPositionID.HasValue || source.PostPositionID.Value == Guid.Empty)
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_JOB_POSITION_NOT_FOUND));

            if (source.PostID != requestedPostId)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_JOB_MISMATCH));

            if (!source.DateOfWork.HasValue || source.DateOfWork.Value != requestedDateOfWork)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_DATE_MISMATCH));

            if (source.PostPositionStatus != PostPositionStatus.Assigned &&
                source.PostPositionStatus != PostPositionStatus.SplitDecisionPending &&
                source.PostPositionStatus != PostPositionStatus.CancellationPending)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_SOURCE_STATUS_INVALID));

            if (source.EmployeeID == targetEmployeeId)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.TRANSFER_TARGET_SAME_AS_SOURCE));

            return source;
        }
        /// <summary>
        /// Bulk inserts or updates multiple post positions with validation.
        /// Overrides base implementation to preserve EmployeeID from DTOs (not override with token employee ID).
        /// </summary>
        /// <param name="dtos">The post position DTOs to insert or update.</param>
        /// <returns>A collection of upserted post position DTOs.</returns>
        public override async Task<IEnumerable<PostPositionDto>> BulkUpsertAsync(IEnumerable<PostPositionDto> dtos, bool isSuperUser = false)
        {
            if (dtos == null)
                throw new ArgumentNullException(nameof(dtos));

            var dtoList = dtos.ToList();
            if (!dtoList.Any())
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_DTOS_EMPTY),
                    nameof(dtos));

            // Validate each DTO
            foreach (var dto in dtoList)
            {
                ValidateDtoParameter(dto);
                ValidatePlaceholderRules(dto);
                await ValidateDuplicatePlaceholderAssignmentAsync(dto).ConfigureAwait(false);
                await UpsertPlaceholderAndAssignIdAsync(dto).ConfigureAwait(false);

                // Match UpsertAsync: only block duplicate active assignments (ignore cancelled/declined/etc.)
                if (dto.PostPositionStatus == PostPositionStatus.Assigned && !dto.IsPlaceholder && dto.DateOfWork.HasValue)
                {
                    await ValidateCompositeKeyDuplicateAsync(
                        dto: dto,
                        filterExpression: e =>
                            e.PostID == dto.PostID &&
                            e.EmployeeID == dto.EmployeeID &&
                            e.DateOfWork == dto.DateOfWork.Value &&
                            e.PostPositionStatus == PostPositionStatus.Assigned &&
                            e.StartTime == dto.PostStartTime &&
                            e.EndTime == dto.PostEndTime,
                        errorMessage: ModelValidationErrorCodes.DUPLICATE_JOB_POSITION_ASSIGNMENT,
                        isEmployeeScoped: false);
                }
            }

            // Check for duplicates within the batch itself (same post, employee, date, and shift times)
            var duplicatesInBatch = dtoList
                .Where(d => d.PostPositionStatus == PostPositionStatus.Assigned && !d.IsPlaceholder && d.DateOfWork.HasValue)
                .GroupBy(d => new { d.PostID, d.EmployeeID, d.DateOfWork, d.PostStartTime, d.PostEndTime })
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicatesInBatch.Any())
            {
                var duplicateInfo = duplicatesInBatch.Select(g =>
                    $"PostID: {g.Key.PostID}, EmployeeID: {g.Key.EmployeeID}, DateOfWork: {g.Key.DateOfWork}");
                throw new ArgumentException(
                    $"Duplicate PostPosition assignments found in the batch: {string.Join("; ", duplicateInfo)}");
            }

            // Store original EmployeeIDs by index (base.BulkUpsertAsync will override them)
            // We use index-based matching since multiple employees can have same PostID + DateOfWork
            var originalEmployeeIds = dtoList
                .Select((d, index) => new { Index = index, EmployeeId = d.EmployeeID })
                .ToList();

            // All validations passed, proceed with bulk upsert (base class will override EmployeeIDs with token)
            var results = (await base.BulkUpsertAsync(dtoList, isSuperUser)).ToList();

            // Restore original EmployeeIDs in results and update database
            // Match by index since base.BulkUpsertAsync preserves order
            var entitiesToUpdate = _mapper.Map<IEnumerable<PostPosition>>(results).ToList();

            // Restore EmployeeIDs by matching index (order is preserved by base.BulkUpsertAsync)
            for (int i = 0; i < entitiesToUpdate.Count && i < originalEmployeeIds.Count; i++)
            {
                if (originalEmployeeIds[i].EmployeeId != Guid.Empty)
                {
                    entitiesToUpdate[i].EmployeeID = originalEmployeeIds[i].EmployeeId;
                }
            }

            // Update entities in database with correct EmployeeIDs
            await _repository.BulkUpsertAsync(entitiesToUpdate, "PostPositionID");

            // Return updated results
            return _mapper.Map<IEnumerable<PostPositionDto>>(entitiesToUpdate);
        }

        /// <summary>
        /// Declines a split post position, records history, and creates a new available post position.
        /// </summary>
        public async Task<PostPositionDto> DeclineSplitAsync(Guid postPositionId)
        {
            try
            {
                if (postPositionId == Guid.Empty)
                    throw new ArgumentException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_ID_REQUIRED));

                var declined = await GetByIdAsync(postPositionId);
                if (declined == null)
                    throw new KeyNotFoundException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));

                await UpdateAsync(
                    filter: x => x.PostPositionID == postPositionId,
                    updateAction: entity =>
                    {
                        entity.PostPositionStatus = PostPositionStatus.Declined;
                        entity.IsSplit = true;
                    });

                var declinedPostPositionId = declined.PostPositionID ?? postPositionId;
                var parentId = declined.ParentPostPositionId ?? declined.SplitPostPositionID ?? declinedPostPositionId;
                Guid? relatedSplitId = declined.SplitPostPositionID ?? declined.ParentPostPositionId;

                var splitCandidates = await GetAllAsync(
                    filter: x =>
                        x.PostID == declined.PostID &&
                        x.IsSplit &&
                        x.PostPositionStatus == PostPositionStatus.ClosedAndSplit &&
                        (x.PostPositionID == parentId ||
                         x.PostPositionID == relatedSplitId ||
                         x.ParentPostPositionId == parentId ||
                         x.ParentPostPositionId == relatedSplitId),
                    includes: null,
                    pageParam: null);

                var splitSource = splitCandidates
                    .OrderByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                    .ThenByDescending(x => x.PostPositionID)
                    .FirstOrDefault();

                if (splitSource == null)
                {
                    var parent = await GetByIdAsync(parentId);
                    if (parent == null)
                        throw new KeyNotFoundException(
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_PARENT_NOT_FOUND_BY_ID, parentId));
                    splitSource = parent;
                }

                var newPostPosition = new PostPositionDto(
                    source: declined,
                    employeeId: splitSource.EmployeeID,
                    status: PostPositionStatus.Assigned,
                    parentPostPositionId: parentId,
                    splitPostPositionId: declinedPostPositionId,
                    isSplit: true);

                // Send "Post Assignment Rejected" email to the splitter.
                await SendSplitRejectedEmailToSplitterAsync(declined);

                // Best-effort push notification to splitter on decline.
                if (parentId != Guid.Empty)
                {
                    var splitterId = await GetSplitterEmployeeIdAsync(parentId);
                    if (splitterId.HasValue
                        && splitterId.Value != Guid.Empty
                        && !IsSelfSplit(splitterId.Value, declined.EmployeeID))
                    {
                        var title = "Job assignment declined";
                        var body = "Your split job assignment has been declined.";
                        await _pushNotificationService.SendToEmployeesAsync(
                            new[] { splitterId.Value },
                            title,
                            body,
                            CancellationToken.None);
                    }
                }

                var result = await UpsertAsync(newPostPosition);
                await _postWorkflowAuditService.LogSuccessAsync(
                    PostPositionStatus.Declined,
                    PostWorkflowAuditContext.FromResult(result, postId: result.PostID));
                if (declined.PostPositionID.HasValue && declined.PostPositionID.Value != Guid.Empty)
                {
                    await _postPositionFlowAuditService.LogSplitDeclinedFlowAsync(
                        declined,
                        parentId,
                        CancellationToken.None).ConfigureAwait(false);
                }
                return result;
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    PostPositionStatus.Declined,
                    ex,
                    PostWorkflowAuditContext.ForPostPosition(postPositionId));
                throw;
            }
        }

        /// <summary>
        /// Gets the employee ID from token with validation.
        /// </summary>
        /// <returns>The employee ID.</returns>
        /// <exception cref="UnauthorizedAccessException">Thrown when employee ID is not found in token.</exception>
        private Guid? GetValidatedEmployeeId()
        {
            var employeeId = _tokenParser.GetEmployeeID();
            if (employeeId == Guid.Empty)
            {
                throw new UnauthorizedAccessException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_EMPLOYEE_ID_NOT_FOUND_IN_TOKEN));
            }
            return employeeId;
        }

        /// <summary>
        /// Calculates the current department week range (start and end dates) based on
        /// the configured <c>OnDutyStartDayOfWeek</c>. Falls back to Sunday when policy is missing.
        /// </summary>
        private (DateOnly WeekStart, DateOnly WeekEnd) GetCurrentDepartmentWeekRange()
        {
            // Use department-local "today" so that week boundaries respect the department's time zone.
            var (departmentToday, _) = GetDateAsPerDepartmentTimeZone(DateOnly.FromDateTime(DateTime.UtcNow));
            var departmentPolicy = _tokenParser.GetDepartmentPolicy();

            DayOfWeek weekStartDay;
            if (departmentPolicy != null)
            {
                try
                {
                    weekStartDay = departmentPolicy.OnDutyStartDayOfWeek.ToDayOfWeek();
                }
                catch
                {
                    weekStartDay = DayOfWeek.Sunday;
                }
            }
            else
            {
                weekStartDay = DayOfWeek.Sunday;
            }

            // Week is defined dynamically from department's configured start day.
            return departmentToday.GetDepartmentWeekRange(weekStartDay);
        }

        public async Task<PagedList<PostTypeResponseDto>> GetWeeklyDashboardPostsAsync(bool isOngoing, PageParam? pagination = null)
        {
            var effectivePageParam = pagination ?? new PageParam();
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortBy))
                effectivePageParam.SortBy = "DateOfWork";
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortDirection))
                effectivePageParam.SortDirection = "asc";
            var (weekStart, weekEnd) = GetCurrentDepartmentWeekRange();

            // Reuse the existing logic that builds "scheduled" vs "ongoing" for Applied (and includes Split/Transferred automatically).
            // We fetch both sets with a large pagesize then apply week filtering + paging here for correct weekly counts.
            const int dashboardFetchMax = 10000;
            var fetchAll = new PageParam { Page = 1, PageSize = dashboardFetchMax };

            var result = await GetForLoggedInEmployeeByStatusesAsync(
                statuses: new[] { PostPositionStatus.Assigned },
                isOngoing: isOngoing,
                pagination: fetchAll);

            // Week filter (department week boundaries)
            var weekly = result.Data
                .Where(d => d.DateOfWork.HasValue)
                .Where(d => d.DateOfWork!.Value >= weekStart && d.DateOfWork!.Value <= weekEnd)
                .ToList();

            // Apply paging after weekly filter
            var ordered = weekly.OrderBy(x => x.DateOfWork).ToList();
            var currentPage = effectivePageParam.Page.GetValueOrDefault(PageParam.MinPage);
            var pageSize = effectivePageParam.PageSize.GetValueOrDefault(PageParam.MinPageSize);
            var skip = (currentPage - 1) * pageSize;

            var paged = ordered.Skip(skip).Take(pageSize).ToList();
            return new PagedList<PostTypeResponseDto>(paged, effectivePageParam, ordered.Count);
        }

        public async Task<PagedList<PostTypeResponseDto>> GetRecentCancelledPostsAsync(PageParam? pagination = null)
        {
            var employeeId = GetValidatedEmployeeId();
            var nowUtc = DateTime.UtcNow;
            var sinceUtc = nowUtc.AddHours(-72);
            var effectivePageParam = pagination ?? new PageParam();
            effectivePageParam.SortBy = "DateOfWork";
            effectivePageParam.SortDirection = "desc";

            var includes = new Expression<Func<PostPosition, object>>[]
            {
                x => x.Post,
                x => x.Employee
            };

            // Use cancellation timestamp from EditedDate when available, otherwise CreatedDate.
            var positions = await GetAllAsync(
                filter: x =>
                    x.EmployeeID == employeeId &&
                    x.PostPositionStatus == PostPositionStatus.Cancelled &&
                    ((x.EditedDate.HasValue && x.EditedDate.Value >= sinceUtc && x.EditedDate.Value <= nowUtc) ||
                     (!x.EditedDate.HasValue && x.CreatedDate.HasValue && x.CreatedDate.Value >= sinceUtc && x.CreatedDate.Value <= nowUtc)),
                includes: includes,
                pageParam: null);

            var ordered = positions
                .OrderByDescending(p => p.DateOfWork)
                .ToList();

            var currentPage = effectivePageParam.Page.GetValueOrDefault(PageParam.MinPage);
            var pageSize = effectivePageParam.PageSize.GetValueOrDefault(PageParam.MinPageSize);
            var skip = (currentPage - 1) * pageSize;

            var paged = ordered
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            var dto = _mapper.Map<List<PostTypeResponseDto>>(paged);

            return new PagedList<PostTypeResponseDto>(dto, effectivePageParam, ordered.Count);
        }

        public async Task<PagedList<PostTypeResponseDto>> GetCloseAssignmentsForLoggedInEmployeeAsync(
            string? searchTerm,
            PageParam? pagination,
            Guid? employeeId,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            var effectivePageParam = pagination ?? new PageParam();
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortBy))
                effectivePageParam.SortBy = "DateOfWork";
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortDirection))
                effectivePageParam.SortDirection = "asc";

            var fixedStatuses = new[]
            {
                PostPositionStatus.Closed,
                PostPositionStatus.ClosedAndSplit,
                PostPositionStatus.Unclosed
            };

            var hasExplicitEmployeeId = employeeId.HasValue && employeeId.Value != Guid.Empty;
            var shouldFilterByEmployee = hasExplicitEmployeeId || false; // useLoggedInEmployeeWhenEmployeeIdMissing = false
            var effectiveEmployeeId = hasExplicitEmployeeId ? employeeId : null;

            // Department filter is resolved internally from token.
            var effectiveDepartmentId = _tokenParser.GetDepartmentID();
            var hasDepartmentFilter = effectiveDepartmentId.HasValue && effectiveDepartmentId.Value != Guid.Empty;

            // Build base filter for status, employee, department, and date range
            Expression<Func<PostPosition, bool>> baseFilter = x =>
                fixedStatuses.Contains(x.PostPositionStatus) &&
                !x.IsPlaceholder &&
                (!shouldFilterByEmployee || x.EmployeeID == effectiveEmployeeId) &&
                (!hasDepartmentFilter || (x.Post != null && x.Post.Request != null && x.Post.Request.DepartmentID == effectiveDepartmentId)) &&
                (!startDate.HasValue || x.DateOfWork >= startDate.Value) &&
                (!endDate.HasValue || x.DateOfWork <= endDate.Value);

            // Build search predicate builder for Post.PostTitle only
            Func<string, Expression<Func<PostPosition, bool>>> searchPredicateBuilder = (term) =>
            {
                var searchTermLower = term.ToLower();
                return x => 
                    (x.Post != null && x.Post.PostTitle != null && EF.Functions.Like(x.Post.PostTitle, $"%{searchTermLower}%"));
            };

            // Include required navigation properties
            var includes = new Expression<Func<PostPosition, object>>[]
            {
                x => x.Post,
                x => x.Employee,
                x => x.Post.Request
            };

            // Use SearchPagedAsync for database-level filtering
            var searchResult = await SearchPagedAsync(
                searchTerm: searchTerm,
                baseFilter: baseFilter,
                searchPredicateBuilder: searchPredicateBuilder,
                pageParam: effectivePageParam,
                includes: includes);

            // Map to PostTypeResponseDto and apply additional business logic
            var postPositionDtos = _mapper.Map<List<PostPositionDto>>(searchResult.Data);
            
            // Apply DateOfWork sorting (already handled by pageParam but ensure consistency)
            var ordered = postPositionDtos.OrderBy(e => e.DateOfWork).ToList();
            
            // Map to final DTO type
            var dtos = _mapper.Map<List<PostTypeResponseDto>>(ordered);

            return new PagedList<PostTypeResponseDto>(dtos, effectivePageParam, searchResult.TotalRecords);
        }

        /// <summary>
        /// Gets post positions for the currently logged-in employee.
        /// Supports three views:
        /// 1) Scheduled posts (status = Applied, isOngoing = false => Applied but NOT ongoing)
        /// 3) Closed posts (status = Closed)
        /// </summary>
        /// <param name="status">The post position status to filter by.</param>
        /// <param name="isOngoing">When true and status is Applied, returns only ongoing posts based on date/time.</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A paginated list of post positions for the logged-in employee.</returns>
        public async Task<PagedList<PostTypeResponseDto>> GetForLoggedInEmployeeAsync(
            PostPositionStatus status,
            bool isOngoing = false,
            [FromQuery] PageParam? pagination = null)
        {
            return await GetForLoggedInEmployeeByStatusesAsync(new[] { status }, isOngoing, pagination);
        }

        /// <inheritdoc />
        public async Task<PagedList<PostTypeResponseDto>> GetForLoggedInEmployeeByStatusesAsync(
            IReadOnlyList<PostPositionStatus> statuses,
            bool isOngoing = false,
            PageParam? pagination = null,
            Guid? employeeId = null,
            bool useLoggedInEmployeeWhenEmployeeIdMissing = true,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? searchTerm = null,
            bool isPlaceholderOnly = false,
            SearchModel? searchModel = null,
            Guid? requesterId = null)
        {
            var hasExplicitEmployeeId = employeeId.HasValue && employeeId.Value != Guid.Empty;
            var shouldFilterByEmployee = hasExplicitEmployeeId || useLoggedInEmployeeWhenEmployeeIdMissing;
            var effectiveEmployeeId = hasExplicitEmployeeId? employeeId : (useLoggedInEmployeeWhenEmployeeIdMissing ? GetValidatedEmployeeId() : null);
            var filterByRequester = requesterId.HasValue && requesterId.Value != Guid.Empty;

            // Department filter is resolved internally from token.
            var effectiveDepartmentId = _tokenParser.GetDepartmentID();
            var hasDepartmentFilter = effectiveDepartmentId.HasValue && effectiveDepartmentId.Value != Guid.Empty;
            var effectivePageParam = pagination ?? new PageParam();
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortBy))
                effectivePageParam.SortBy = "DateOfWork";
            if (string.IsNullOrWhiteSpace(effectivePageParam.SortDirection))
                effectivePageParam.SortDirection = "asc";

            if (statuses == null || statuses.Count == 0)
                return new PagedList<PostTypeResponseDto>(new List<PostTypeResponseDto>(), effectivePageParam, 0);

            var statusSet = statuses.Distinct().ToHashSet();

            // Check for outstanding ready-to-close posts policy
            

            // When caller asks ONLY CancellationPending, return rows assigned to other employees.
            if (statusSet.Count == 1 && statusSet.Contains(PostPositionStatus.CancellationPending))
            {
                var departmentPolicy = _tokenParser.GetDepartmentPolicy();
                if (departmentPolicy != null && departmentPolicy.EnablePolicyManagement && shouldFilterByEmployee && effectiveEmployeeId.HasValue)
                {
                    var employeeStatistics = await GetEmployeeStatistics(departmentPolicy, effectiveEmployeeId.Value);
                    if (employeeStatistics.HasOutstandingReadyToClosePost(departmentPolicy))
                    {
                        return new PagedList<PostTypeResponseDto>(new List<PostTypeResponseDto>(), effectivePageParam, 0)
                        {
                            Error = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB),
                            HasOutstandingReadyToClosePost = true
                        };
                    }
                }
                return await GetOnlyCancellationPendingForOtherEmployeesAsync(
                    effectivePageParam,
                    hasDepartmentFilter,
                    effectiveDepartmentId,
                    effectiveEmployeeId,
                    searchModel);
            }

            // For PostReadyToClose/closed views, include unclosed posts in the same list.
            var includes = DefaultDetailIncludes;

            var fetchAllPageParam = new PageParam
            {
                Offset = 0,
                Limit = int.MaxValue,
                SortBy = "DateOfWork",
                SortDirection = "asc"
            };

            var entitiesPaged = await GetAllPagedAsync(
                filter: x =>
                    (isPlaceholderOnly ? x.IsPlaceholder : !x.IsPlaceholder) &&
                    (!shouldFilterByEmployee || isPlaceholderOnly || x.EmployeeID == effectiveEmployeeId) &&
                    (!hasDepartmentFilter || (x.Post != null && x.Post.Request != null && x.Post.Request.DepartmentID == effectiveDepartmentId)) &&
                    (!filterByRequester || (x.Post != null && x.Post.Request != null && x.Post.Request.RequesterID == requesterId!.Value)) &&
                    statusSet.Contains(x.PostPositionStatus),
                pageParam: fetchAllPageParam,
                includes: includes).ConfigureAwait(false);

            if (entitiesPaged.TotalRecords == 0)
                return new PagedList<PostTypeResponseDto>(new List<PostTypeResponseDto>(), effectivePageParam, 0);

            var entities = _mapper.Map<List<PostPositionDto>>(entitiesPaged.Data);
            var isAssignedFlow = statusSet.Contains(PostPositionStatus.Assigned);
            List<PostPositionDto> resultRows;
            if (isAssignedFlow)
            {
                var departmentNow = GetCurrentDepartmentDateTime();
                resultRows = entities
                    .Where(x => x.MatchesOngoingView(isOngoing, departmentNow))
                    .ToList();

                if (isOngoing && resultRows.Any())
                    await PromoteDueCancellationPendingToAppliedAsync(shouldFilterByEmployee ? effectiveEmployeeId : null);

                resultRows = await RemoveRetakenCancellationPendingAsync(resultRows);
            }
            else
            {
                // Non-assigned flows (e.g. closed-assignments) should not be split by ongoing/scheduled time.
                resultRows = entities;
            }

            if (isOngoing)
            {
                resultRows = ApplyOngoingOnlyFilters(resultRows, startDate, endDate, searchTerm);
            }

            var ordered = resultRows.OrderBy(e => e.DateOfWork).ToList();
            var currentPage = effectivePageParam.Page.GetValueOrDefault(PageParam.MinPage);
            var currentPageSize = effectivePageParam.PageSize.GetValueOrDefault(PageParam.MinPageSize);
            var skip = (currentPage - 1) * currentPageSize;
            var paged = ordered.Skip(skip).Take(currentPageSize).ToList();
            var dtos = _mapper.Map<List<PostTypeResponseDto>>(paged);

            var currentEmployeeId = shouldFilterByEmployee ? effectiveEmployeeId : null;

            if (dtos.Any())
            {
                var grouped = paged
                    .Where(x => x.DateOfWork.HasValue)
                    .GroupBy(x => new { x.PostID, x.DateOfWork });

                foreach (var group in grouped)
                {
                    var sameDayPositions = await GetPostPositionsWithEmployeeAllStatusesAsync(
                        group.Key.PostID,
                        group.Key.DateOfWork,
                        pagination: null);

                    if (!sameDayPositions.Any())
                        continue;

                    var postPositions = _mapper.Map<List<PostPositionDto>>(sameDayPositions);

                    var groupDtos = dtos
                        .Where(d => d.PostID == group.Key.PostID && d.DateOfWork == group.Key.DateOfWork)
                        .ToList();

                    await ApplyVehicleEquipmentVisibilityAsync(
                        groupDtos,
                        postPositions,
                        currentEmployeeId);
                }
            }
            return new PagedList<PostTypeResponseDto>(dtos, effectivePageParam, ordered.Count);
        }

        public async Task<PagedList<PostTypeResponseDto>> GetOngoingPostPositionsAsync(
            PageParam? pagination = null,
            Guid? employeeId = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            string? searchTerm = null,
            bool useLoggedInEmployeeWhenEmployeeIdMissing = false,
            bool isPlaceholderOnly = false,
            Guid? requesterId = null)
        {
            return await GetForLoggedInEmployeeByStatusesAsync(
                statuses: new[] { PostPositionStatus.Assigned, PostPositionStatus.SplitDecisionPending, PostPositionStatus.Unclosed },
                isOngoing: true,
                pagination: pagination,
                employeeId: employeeId,
                useLoggedInEmployeeWhenEmployeeIdMissing: useLoggedInEmployeeWhenEmployeeIdMissing,
                startDate: startDate,
                endDate: endDate,
                searchTerm: searchTerm,
                isPlaceholderOnly: isPlaceholderOnly,
                requesterId: requesterId);
        }

        public async Task<PagedList<OfficerScheduledAssignmentDto>> GetOfficerScheduleOverviewAsync(
            string? searchTerm,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    "The current token does not include a department ID (DepartmentID claim).");
            }

            var departmentNow = GetCurrentDepartmentDateTime();
            var page = pagination ?? new PageParam();
            var minDate = DateOnly.FromDateTime(departmentNow.AddDays(-1));
            var maxDate = DateOnly.FromDateTime(departmentNow.AddDays(1));

            var positions = await _repository
                .GetOfficerSchedulePostPositionsAsync(
                    departmentId.Value,
                    minDate,
                    maxDate,
                    cancellationToken)
                .ConfigureAwait(false);

            var positionDtos = _mapper.Map<List<PostPositionDto>>(positions);

            var rows = positionDtos
                .Where(e => e.Employee != null)
                .Select(e => TryClassifyAssignment(e, departmentNow))
                .OfType<OfficerScheduledAssignmentDto>()
                .ToList();

            IEnumerable<OfficerScheduledAssignmentDto> filtered = rows;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var q = searchTerm.Trim();
                filtered = filtered.Where(x =>
                    (!string.IsNullOrWhiteSpace(x.EmployeeName) && x.EmployeeName.Contains(q, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrWhiteSpace(x.EmployeeType) && x.EmployeeType.Contains(q, StringComparison.OrdinalIgnoreCase))
                    || (!string.IsNullOrWhiteSpace(x.Post) && x.Post.Contains(q, StringComparison.OrdinalIgnoreCase)));
            }

            var ordered = filtered
                .OrderBy(x => x.DateOfWork)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.EmployeeName)
                .ToList();

            var skip = page.Offset!.Value;
            var take = page.Limit!.Value;
            var pagedData = ordered.Skip(skip).Take(take).ToList();

            return new PagedList<OfficerScheduledAssignmentDto>(pagedData, page, ordered.Count);
        }

        private OfficerScheduledAssignmentDto? TryClassifyAssignment(PostPositionDto positionDto, DateTime departmentNow)
        {
            var status = positionDto.GetOfficerScheduleStatus(departmentNow, recentlyEndedMinutes: 50, upcomingHours: 8);
            if (!status.HasValue)
                return null;

            var end = positionDto.GetPostPositionEndDateTime();

            var employeeDto = positionDto.Employee;
            if (employeeDto == null)
                return null;

            var assignmentDto = _mapper.Map<PostTypeResponseDto>(positionDto);

            var row = new OfficerScheduledAssignmentDto(
                employeeName: employeeDto.GetFullName(),
                payroll: employeeDto.EmployeeNumber,
                employeeType: employeeDto.EmployeeType?.EmployeeTypeShort,
                post: assignmentDto.PostTitle,
                dateOfWork: assignmentDto.DateOfWork,
                startTime: assignmentDto.PostStartTime,
                endTime: end.HasValue ? TimeOnly.FromDateTime(end.Value) : assignmentDto.PostEndTime,
                callSign: assignmentDto.CallSign,
                hoursWorked: assignmentDto.PostLength > 0 ? assignmentDto.PostLength / 60m : null);

            row.Status = status.Value;
            return row;
        }

        private static List<PostPositionDto> ApplyOngoingOnlyFilters(
            List<PostPositionDto> rows,
            DateOnly? startDate,
            DateOnly? endDate,
            string? searchTerm)
        {
            var filtered = rows;

            if (startDate.HasValue)
                filtered = filtered.Where(x => x.DateOfWork.HasValue && x.DateOfWork.Value >= startDate.Value).ToList();

            if (endDate.HasValue)
                filtered = filtered.Where(x => x.DateOfWork.HasValue && x.DateOfWork.Value <= endDate.Value).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedSearch = searchTerm.Trim().ToLowerInvariant();
                filtered = filtered
                    .Where(x => IsOngoingRowMatchBySearch(x, normalizedSearch))
                    .ToList();
            }

            return filtered;
        }

        private static bool IsOngoingRowMatchBySearch(PostPositionDto row, string normalizedSearch)
        {
            var hasTitleMatch =
                !string.IsNullOrWhiteSpace(row.Post?.PostTitle) &&
                row.Post.PostTitle.ToLowerInvariant().Contains(normalizedSearch);

            var requesterName = row.Post?.Request?.Requester?.Name;
            var hasRequesterMatch =
                !string.IsNullOrWhiteSpace(requesterName) &&
                requesterName.ToLowerInvariant().Contains(normalizedSearch);

            var employee = row.Employee;
            var hasEmployeeMatch = employee != null && (
                (!string.IsNullOrWhiteSpace(employee.FirstName)
                    && employee.FirstName.ToLowerInvariant().Contains(normalizedSearch))
                || (!string.IsNullOrWhiteSpace(employee.LastName)
                    && employee.LastName.ToLowerInvariant().Contains(normalizedSearch))
                || (!string.IsNullOrWhiteSpace(employee.GetFullName())
                    && employee.GetFullName().ToLowerInvariant().Contains(normalizedSearch)));

            var hasDurationMatch = row.LengthOfWork.ToString().Contains(normalizedSearch);

            return hasTitleMatch || hasRequesterMatch || hasEmployeeMatch || hasDurationMatch;
        }

        private (DateOnly? StartDate, DateOnly? EndDate) ResolveSearchModelDateRange(SearchModel? searchModel)
        {
            var (searchStartDate, _) = GetDateAsPerDepartmentTimeZone(searchModel.StartDate);
            var searchEndDate = DateHelper.GetEndDate(searchStartDate, searchModel.NumberOfDays);
            return (searchStartDate, searchEndDate);
        }

        private List<PostPositionDto> ApplySearchModelFiltersToPostPositionRows(
            IEnumerable<PostPositionDto> rows,
            SearchModel? searchModel,
            bool applyKeyFilters,
            bool skipReleaseAsFilter = false)
        {
            var (searchStartDate, searchEndDate) = ResolveSearchModelDateRange(searchModel);
            IEnumerable<PostPositionDto> filtered = rows;

            if (applyKeyFilters && searchStartDate.HasValue && searchEndDate.HasValue)
            {
                filtered = filtered.Where(x =>
                    x.DateOfWork.HasValue &&
                    x.DateOfWork.Value >= searchStartDate.Value &&
                    x.DateOfWork.Value <= searchEndDate.Value);
            }

            if (applyKeyFilters && searchModel.Request is { } requestId && requestId != Guid.Empty)
            {
                filtered = filtered.Where(x =>
                    x.Post != null &&
                    (x.Post.RequestID == requestId ||
                     (x.Post.Request != null && x.Post.Request.RequestID == requestId)));
            }

            if (applyKeyFilters && searchModel.Requester is { } requesterId && requesterId != Guid.Empty)
            {
                filtered = filtered.Where(x =>
                    x.Post?.Request != null && x.Post.Request.RequesterID == requesterId);
            }

            if (applyKeyFilters && searchModel.PayCode is { } payCodeId && payCodeId != Guid.Empty)
            {
                filtered = filtered.Where(x =>
                    x.Post != null &&
                    (x.Post.PayCodeID == payCodeId ||
                     (x.Post.PayCode != null && x.Post.PayCode.PayCodeID == payCodeId)));
            }
            return filtered.ToList();
        }

        public async Task ApplyVehicleEquipmentVisibilityAsync(
            List<PostTypeResponseDto> dtos,
            List<PostPositionDto> allSameDayPositions,
            Guid? currentEmployeeId)
        {
            if (dtos == null || dtos.Count == 0)
                return;

            if (!currentEmployeeId.HasValue || currentEmployeeId.Value == Guid.Empty)
                return;

            if (allSameDayPositions == null || allSameDayPositions.Count == 0)
                return;

            var viewerEmployeeId = currentEmployeeId.Value;
            var scopeCache = new Dictionary<(Guid RequestId, DateOnly? DateOfWork), VehicleEquipmentScopeData>();
            var requestRequiresCache = new Dictionary<Guid, (bool RequiresVehicle, bool RequiresEquipment)>();

            var postIds = allSameDayPositions
                .Select(p => p.PostID)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();
            var workDates = allSameDayPositions
                .Where(p => p.DateOfWork.HasValue)
                .Select(p => p.DateOfWork!.Value)
                .Distinct()
                .ToList();
            var allocationCounts = await _postPositionVehicleRepository
                .GetAllocationCountsByPostAndDateAsync(postIds, workDates)
                .ConfigureAwait(false);

            var isFinalizedByPositionId = allSameDayPositions
                .Where(p => p.PostPositionID.HasValue && p.PostPositionID.Value != Guid.Empty)
                .GroupBy(p => p.PostPositionID!.Value)
                .ToDictionary(g => g.Key, g => g.Any(p => p.IsAllocationFinalized));

            foreach (var dto in dtos)
            {
                dto.ShowVehicle = false;
                dto.ShowEquipment = false;

                var isAllocationFinalized = dto.IsAllocationFinalized
                    || (dto.PostPositionID is Guid positionId
                        && positionId != Guid.Empty
                        && isFinalizedByPositionId.TryGetValue(positionId, out var finalized)
                        && finalized);

                // Frozen / finalized allocations stay locked to PostPositionVehicle (even when empty).
                if (await TryApplyAllocatedEquipmentVisibilityAsync(dto, isAllocationFinalized).ConfigureAwait(false))
                    continue;

                if (!dto.RequestID.HasValue || dto.RequestID.Value == Guid.Empty)
                    continue;

                var requestId = dto.RequestID.Value;
                var cacheKey = (requestId, dto.DateOfWork);

                if (!requestRequiresCache.TryGetValue(requestId, out var requestRequires))
                {
                    var request = await _requestService.GetByIdAsync(requestId).ConfigureAwait(false);
                    requestRequires = (
                        request?.RequiresVehicle ?? false,
                        request?.RequiresEquipment ?? false);
                    requestRequiresCache[requestId] = requestRequires;
                }

                if (!scopeCache.TryGetValue(cacheKey, out var scope))
                {
                    var loaded = await _viewDetailsEquipmentRepository
                        .LoadAsync(requestId, postId: null, dto.DateOfWork, loadRankContext: true)
                        .ConfigureAwait(false);

                    scope = new VehicleEquipmentScopeData
                    {
                        RequestEquipment = _mapper.Map<List<RequestEquipmentDto>>(loaded.RequestEquipment),
                        PostEquipmentByPostId = loaded.PostEquipment
                            .GroupBy(pe => pe.PostID)
                            .ToDictionary(g => g.Key, g => _mapper.Map<List<PostEquipmentDto>>(g.ToList())),
                        PostDepartmentEquipmentIdsByPostId = loaded.PostDepartmentEquipmentIdsByPostId,
                        RequestPositions = _mapper.Map<List<PostPositionDto>>(loaded.RequestPositions),
                        HireDateByEmployeeId = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(loaded.EmployeeTenures),
                        EmployeeTenures = loaded.EmployeeTenures
                    };
                    scopeCache[cacheKey] = scope;
                }

                var postEquipment = scope.PostEquipmentByPostId.TryGetValue(dto.PostID, out var postList)
                    ? postList
                    : new List<PostEquipmentDto>();

                if (postEquipment.Count == 0)
                    continue;

                scope.PostDepartmentEquipmentIdsByPostId.TryGetValue(dto.PostID, out var postOverrideDepartmentEquipmentIds);

                var (showVehicle, showEquipment) = ViewDetailsEquipmentLines.ComputeShowFlagsForViewer(
                    viewerEmployeeId,
                    scope.RequestEquipment,
                    postEquipment,
                    scope.RequestPositions,
                    allSameDayPositions,
                    dto.PostRequiresVehicle,
                    dto.PostRequiresEquipment,
                    requestRequires.RequiresVehicle,
                    requestRequires.RequiresEquipment,
                    scope.HireDateByEmployeeId,
                    dto.DateOfWork,
                    postOverrideDepartmentEquipmentIds,
                    scope.EmployeeTenures,
                    allocationCounts: allocationCounts);

                dto.ShowVehicle = showVehicle;
                dto.ShowEquipment = showEquipment;
            }
        }

        /// <summary>
        /// Applies ShowVehicle/ShowEquipment from <c>PostPositionVehicle</c> when rows exist.
        /// When <paramref name="isAllocationFinalized"/> is true and no rows exist, hides both and
        /// returns true so callers do not fall back to live rank rules.
        /// </summary>
        private async Task<bool> TryApplyAllocatedEquipmentVisibilityAsync(
            PostTypeResponseDto dto,
            bool isAllocationFinalized)
        {
            if (dto.PostPositionID is not Guid rowId || rowId == Guid.Empty)
                return false;

            var names = await _postPositionVehicleRepository
                .GetAssignedEquipmentTypeNamesAsync(rowId)
                .ConfigureAwait(false);

            if (names.Count > 0)
            {
                dto.ShowVehicle = names.Any(n => n.IsVehicle);
                dto.ShowEquipment = names.Any(n => !n.IsVehicle);
                return true;
            }

            if (!isAllocationFinalized)
                return false;

            dto.ShowVehicle = false;
            dto.ShowEquipment = false;
            return true;
        }

        private sealed class VehicleEquipmentScopeData
        {
            public List<RequestEquipmentDto> RequestEquipment { get; init; } = new();
            public Dictionary<Guid, List<PostEquipmentDto>> PostEquipmentByPostId { get; init; } = new();
            public Dictionary<Guid, HashSet<Guid>> PostDepartmentEquipmentIdsByPostId { get; init; } = new();
            public List<PostPositionDto> RequestPositions { get; init; } = new();
            public IReadOnlyDictionary<Guid, DateOnly?> HireDateByEmployeeId { get; init; } =
                new Dictionary<Guid, DateOnly?>();
            public IReadOnlyList<EmployeeTenure> EmployeeTenures { get; init; } = Array.Empty<EmployeeTenure>();
        }

        private async Task<PagedList<PostTypeResponseDto>> GetOnlyCancellationPendingForOtherEmployeesAsync(
            PageParam effectivePageParam,
            bool hasDepartmentFilter,
            Guid? effectiveDepartmentId,
            Guid? subjectEmployeeId,
            SearchModel? searchModel = null)
        {
            var includes = new Expression<Func<PostPosition, object>>[]
            {
                x => x.Post,
                x => x.Post.Request,
                x => x.Post.Request.RequestSchedules,
                x => x.Post.PayCode,
                x => x.Post.EmployeeType,
                x => x.Employee
            };

            // Subject employee's assignments that consume a slot (same statuses used when counting "booked" for available posts).
            List<PostPositionDto> myTakenSlots = new List<PostPositionDto>();
            if (subjectEmployeeId.HasValue && subjectEmployeeId.Value != Guid.Empty)
            {
                myTakenSlots = (await GetAllAsync(
                    filter: x =>
                        x.EmployeeID == subjectEmployeeId.Value &&
                        x.DateOfWork.HasValue &&
                        TakenSlotOccupyingStatuses.Contains(x.PostPositionStatus) &&
                        (!hasDepartmentFilter || (x.Post != null && x.Post.Request != null && x.Post.Request.DepartmentID == effectiveDepartmentId)),
                    includes,
                    pageParam: null).ConfigureAwait(false)).ToList();
            }

            var departmentNow = GetCurrentDepartmentDateTime();
            var allPendingForOthers = (await GetAllAsync(
                filter: x =>
                    x.PostPositionStatus == PostPositionStatus.CancellationPending &&
                    (!subjectEmployeeId.HasValue || subjectEmployeeId.Value == Guid.Empty || x.EmployeeID != subjectEmployeeId.Value) &&
                    (!hasDepartmentFilter || (x.Post != null && x.Post.Request != null && x.Post.Request.DepartmentID == effectiveDepartmentId)),
                includes,
                pageParam: null).ConfigureAwait(false)).ToList();

            var pendingDtos = _mapper.Map<List<PostPositionDto>>(allPendingForOthers);
            // Others may pick up a pending cancel only before the slot start time (hide once shift has started).
            var filtered = pendingDtos
                .Where(p => !p.IsSamePostSlotAsAny(myTakenSlots) &&
                    !p.IsPostPositionDateTimePassed(departmentNow) &&
                    p.Post?.Request?.GetRequestStatusByDate(p.DateOfWork!.Value) == RequestStatus.ReleaseNow)
                .ToList();

            filtered = ApplySearchModelFiltersToPostPositionRows(
                filtered,
                searchModel,
                applyKeyFilters: true,
                skipReleaseAsFilter: true);

            filtered = filtered
                .OrderBy(p => p.DateOfWork)
                .ToList();

            var totalCount = filtered.Count;
            var currentPage = effectivePageParam.Page.GetValueOrDefault(PageParam.MinPage);
            var currentPageSize = effectivePageParam.PageSize.GetValueOrDefault(PageParam.MinPageSize);
            var skip = (currentPage - 1) * currentPageSize;
            var pagedDtos = filtered.Skip(skip).Take(currentPageSize).ToList();

            var dtos = _mapper.Map<List<PostTypeResponseDto>>(pagedDtos);

            return new PagedList<PostTypeResponseDto>(dtos, effectivePageParam, totalCount);
        }

        /// <summary>
        /// Removes CancellationPending rows when the same post/date is fully staffed (available = 0)
        /// and at least one slot has been taken by another employee.
        /// </summary>
        private async Task<List<PostPositionDto>> RemoveRetakenCancellationPendingAsync(List<PostPositionDto> items)
        {
            if (items == null || items.Count == 0)
                return items;

            var pendingRows = items
                .Where(x => x.PostPositionStatus == PostPositionStatus.CancellationPending && x.DateOfWork.HasValue)
                .ToList();

            if (pendingRows.Count == 0)
                return items;

            var minDate = pendingRows.Min(x => x.DateOfWork!.Value);
            var maxDate = pendingRows.Max(x => x.DateOfWork!.Value);
            var postIds = pendingRows.Select(x => x.PostID).Distinct().ToList();

            var takenRows = (await GetAllAsync(
                filter: x =>
                    x.DateOfWork.HasValue &&
                    x.DateOfWork.Value >= minDate &&
                    x.DateOfWork.Value <= maxDate &&
                    postIds.Contains(x.PostID) &&
                    (x.PostPositionStatus == PostPositionStatus.Assigned ||
                     x.PostPositionStatus == PostPositionStatus.SplitDecisionPending),
                includes: null,
                pageParam: null)).ToList();

            var takenByKey = takenRows
                .Where(x => x.DateOfWork.HasValue)
                .GroupBy(x => new { x.PostID, x.DateOfWork })
                .Select(g => new
                {
                    g.Key.PostID,
                    g.Key.DateOfWork,
                    TakenCount = g.Count(),
                    EmployeeIds = g.Select(x => x.EmployeeID).Distinct().ToList()
                })
                .ToList();

            return items
                .Where(x =>
                    !(x.PostPositionStatus == PostPositionStatus.CancellationPending &&
                      x.DateOfWork.HasValue &&
                      x.Post != null &&
                      takenByKey.Any(t =>
                          t.PostID == x.PostID &&
                          t.DateOfWork == x.DateOfWork &&
                          t.TakenCount >= x.Post.TotalPostPositions &&
                          t.EmployeeIds.Any(empId => empId != x.EmployeeID))))
                .ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<Guid>> RevertCancellationPendingToAssignedAsync(
            Guid departmentId,
            DateTime departmentNow,
            CancellationToken cancellationToken = default)
        {
            var dueIds = (await GetPostPositionsPastScheduledStartAsync(
                    departmentId,
                    departmentNow,
                    PostPositionStatus.CancellationPending,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false))
                .Select(x => x.PostPositionID!.Value)
                .Distinct()
                .ToList();

            if (dueIds.Count == 0)
                return Array.Empty<Guid>();

            await UpdateAsync(
                filter: x => dueIds.Contains(x.PostPositionID) && x.PostPositionStatus == PostPositionStatus.CancellationPending,
                updateAction: x => x.PostPositionStatus = PostPositionStatus.Assigned).ConfigureAwait(false);

            return dueIds;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<PostPositionDto>> GetPostPositionsPastScheduledStartAsync(
            Guid departmentId,
            DateTime departmentNow,
            PostPositionStatus? status = null,
            bool excludePlaceholders = false,
            bool requireEmployee = false,
            bool requireRequestId = false,
            IEnumerable<PostPositionStatus>? statuses = null,
            bool onlyUnfinalizedAllocations = false,
            CancellationToken cancellationToken = default)
        {
            var today = DateOnly.FromDateTime(departmentNow);
            var currentTime = TimeOnly.FromDateTime(departmentNow);

            var statusList = statuses?.ToList() ?? (status.HasValue ? new List<PostPositionStatus> { status.Value } : null);

            return (await GetAllAsync(
                filter: x =>
                    x.PostPositionID != Guid.Empty
                    && (statusList == null || statusList.Contains(x.PostPositionStatus))
                    && (!excludePlaceholders || !x.IsPlaceholder)
                    && (!requireEmployee || (x.EmployeeID != null && x.EmployeeID != Guid.Empty))
                    && (!onlyUnfinalizedAllocations || !x.IsAllocationFinalized)
                    && x.DateOfWork.HasValue
                    && x.StartTime.HasValue
                    && x.Post != null
                    && x.Post.Request != null
                    && x.Post.Request.DepartmentID == departmentId
                    && (!requireRequestId || (x.Post.RequestID != null && x.Post.RequestID != Guid.Empty))
                    && (
                        x.DateOfWork.Value < today
                        || (x.DateOfWork.Value == today && x.StartTime.Value <= currentTime)),
                includes: new Expression<Func<PostPosition, object>>[] { x => x.Post!, x => x.Post!.Request! },
                pageParam: null).ConfigureAwait(false)).ToList();
        }

        /// <summary>
        /// Promotes CancellationPending posts to Assigned once their scheduled start time is reached.
        /// </summary>
        private async Task PromoteDueCancellationPendingToAppliedAsync(Guid? employeeId = null)
        {
            var today = GetCurrentDepartmentDate();
            var currentTime = GetCurrentDepartmentTime();

            await UpdateAsync(
                filter: x =>
                    x.PostPositionStatus == PostPositionStatus.CancellationPending &&
                    x.DateOfWork.HasValue &&
                    (!employeeId.HasValue || x.EmployeeID == employeeId.Value) &&
                    (
                        x.DateOfWork.Value < today ||
                        (x.DateOfWork.Value == today &&
                         x.StartTime.HasValue &&
                         x.StartTime.Value <= currentTime)
                    ),
                updateAction: x => x.PostPositionStatus = PostPositionStatus.Assigned).ConfigureAwait(false);
        }



        public async Task<int> UpdateStatusByPostIdAsync(Guid postPositionId, PostPositionStatus status)
        {
            PostPositionDto? existingPostPosition = null;
            try
            {
                // Validate that the PostPosition exists
                existingPostPosition = await GetByIdAsync(postPositionId);
                if (existingPostPosition == null)
                {
                    throw new Exception(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));
                }

                var wasSplitAccept = status == PostPositionStatus.Assigned
                    && existingPostPosition.PostPositionStatus == PostPositionStatus.SplitDecisionPending
                    && existingPostPosition.ParentPostPositionId.HasValue
                    && existingPostPosition.ParentPostPositionId.Value != Guid.Empty;

                // Use the generic service's UpdateAsync method for efficient update
                var updatedCount = await UpdateAsync(
                    filter: x => x.PostPositionID == postPositionId,
                    updateAction: entity =>
                    {
                        entity.PostPositionStatus = status;
                        if (status == PostPositionStatus.Unclosed)
                            entity.WasUnclosed = true;
                    }
                );

                if (updatedCount > 0)
                {
                    await _postWorkflowAuditService.LogSuccessAsync(
                        status,
                        new PostWorkflowAuditContext
                        {
                            PostPositionId = postPositionId,
                            PostId = existingPostPosition.PostID
                        });

                    await _postPositionFlowAuditService.LogStatusUpdateFlowAsync(
                        existingPostPosition,
                        status,
                        CancellationToken.None).ConfigureAwait(false);
                }

                // When split employee accepts, notify splitter and send assign-post email to the accepting employee.
                if (wasSplitAccept && updatedCount > 0)
                {
                    await SendSplitAcceptedEmailToSplitterAsync(existingPostPosition);

                    if (!existingPostPosition.IsPlaceholder
                        && existingPostPosition.DateOfWork.HasValue)
                    {
                        await _requestService.SendAssignedPostNotificationAsync(
                            existingPostPosition.PostID,
                            existingPostPosition.DateOfWork.Value,
                            existingPostPosition.EmployeeID,
                            _tokenParser.GetDepartmentID(),
                            CancellationToken.None);
                    }

                    var splitterId = await GetSplitterEmployeeIdAsync(existingPostPosition.ParentPostPositionId!.Value);
                    if (splitterId.HasValue
                        && splitterId.Value != Guid.Empty
                        && !IsSelfSplit(splitterId.Value, existingPostPosition.EmployeeID))
                    {
                        var title = "Job assignment accepted";
                        var body = "Your split job assignment has been accepted.";
                        await _pushNotificationService.SendToEmployeesAsync(
                            new[] { splitterId.Value },
                            title,
                            body,
                            CancellationToken.None);
                    }
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    status,
                    ex,
                    new PostWorkflowAuditContext
                    {
                        PostPositionId = postPositionId,
                        PostId = existingPostPosition?.PostID
                    });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<int> CancelPostPositionAsync(Guid postPositionId, string? reasonForCancel = null, bool isApproveCancellation = false, CancellationToken cancellationToken = default)
        {
            var targetStatus = isApproveCancellation
                ? PostPositionStatus.Cancelled
                : PostPositionStatus.CancellationPending;
            PostPosition? existingPostPosition = null;

            try
            {
                var departmentPolicy = _tokenParser.GetDepartmentPolicy();
                if (!await IsEmployeeCancellationAllowedAsync().ConfigureAwait(false))
                    throw new ArgumentException("Post cancellation is not allowed for this department.");

                existingPostPosition = await GetPostPositionWithPostByIdAsync(postPositionId);
                if (existingPostPosition == null)
                    throw new InvalidOperationException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));

                var normalizedReason = string.IsNullOrWhiteSpace(reasonForCancel) ? null : reasonForCancel.Trim();
                var updatedCount = await UpdateAsync(
                    filter: x => x.PostPositionID == postPositionId,
                    updateAction: entity =>
                    {
                        entity.PostPositionStatus = targetStatus;
                        entity.ReasonForCancel = normalizedReason;
                    });

                if (updatedCount > 0 && departmentPolicy.ConsiderOpenWaitListOnCancellation)
                {
                    var waitListShiftStartTime = existingPostPosition.StartTime ?? existingPostPosition.Post?.PostStartTime ?? TimeOnly.MinValue;
                    var waitListShiftStartDateTime = existingPostPosition.DateOfWork?.ToDateTime(waitListShiftStartTime) ?? GetCurrentDepartmentDateTime();

                    if (IsTimeGreaterThan72Hours(waitListShiftStartDateTime))
                    {
                        await AssignCancelledPostFromWaitingListAsync(existingPostPosition, isApproveCancellation, cancellationToken);
                    }
                }
                // Send cancellation email to the employee whose post was cancelled (assigned to this post position)
                if (!existingPostPosition.IsPlaceholder && existingPostPosition.EmployeeID != Guid.Empty && !string.IsNullOrWhiteSpace(existingPostPosition.DateOfWork?.ToString()))
                {
                    var postTitle = existingPostPosition.Post?.PostTitle?.Trim() ?? string.Empty;
                    var postAssignedDate = existingPostPosition.DateOfWork?.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
                    var postStartTime = existingPostPosition.StartTime?.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;

                    await SendCancellationEmailsAsync(existingPostPosition, normalizedReason, cancellationToken);

                    // 3) Push notification to employee on cancellation.
                    var title = "Post Cancelled";
                    var pushBody = $"{postTitle} on {postAssignedDate} at {postStartTime} has been cancelled.";
                    await _pushNotificationService.SendToEmployeesAsync(
                        new[] { existingPostPosition.EmployeeID.Value },
                        title,
                        pushBody,
                        cancellationToken);
                }

                if (updatedCount > 0)
                {
                    await _postWorkflowAuditService.LogSuccessAsync(
                        targetStatus,
                        new PostWorkflowAuditContext
                        {
                            PostPositionId = postPositionId,
                            PostId = existingPostPosition.PostID,
                            EmployeeId = existingPostPosition.EmployeeID
                        });

                    await _postPositionFlowAuditService.LogCancellationFlowAsync(
                        postPositionId,
                        existingPostPosition.PostID,
                        existingPostPosition.EmployeeID is { } eid && eid != Guid.Empty ? eid : null,
                        isApproveCancellation,
                        normalizedReason,
                        cancellationToken).ConfigureAwait(false);
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    targetStatus,
                    ex,
                    new PostWorkflowAuditContext
                    {
                        PostPositionId = postPositionId,
                        PostId = existingPostPosition?.PostID,
                        EmployeeId = existingPostPosition?.EmployeeID,
                        FailureReason = ex.GetUserFacingMessage()
                    });
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<int> RevokeCancellationAsync(Guid postPositionId, CancellationToken cancellationToken = default)
        {
            PostPositionDto? existing = null;
            try
            {
                existing = await GetByIdAsync(postPositionId);
                if (existing == null)
                    throw new InvalidOperationException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));

                if (existing.PostPositionStatus != PostPositionStatus.CancellationPending)
                    throw new ArgumentException(ModelErrorCodes.GetErrorMessage(ModelErrorCodes.CANCELLATION_ONLY_WHEN_CANCELLATION_PENDING));

                var updatedCount = await UpdateAsync(
                    filter: x => x.PostPositionID == postPositionId,
                    updateAction: entity =>
                    {
                        entity.PostPositionStatus = PostPositionStatus.Assigned;
                        entity.ReasonForCancel = null;
                    });

                if (updatedCount > 0)
                {
                    await _postWorkflowAuditService.LogSuccessAsync(
                        PostPositionStatus.Assigned,
                        new PostWorkflowAuditContext
                        {
                            PostPositionId = postPositionId,
                            PostId = existing.PostID,
                            EmployeeId = existing.EmployeeID
                        });

                    await _postPositionFlowAuditService.LogCancellationRevokedFlowAsync(
                        postPositionId,
                        existing.PostID,
                        existing.EmployeeID == Guid.Empty ? null : existing.EmployeeID,
                        cancellationToken).ConfigureAwait(false);
                }

                return updatedCount;
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    PostPositionStatus.Assigned,
                    ex,
                    new PostWorkflowAuditContext
                    {
                        PostPositionId = postPositionId,
                        PostId = existing?.PostID,
                        EmployeeId = existing?.EmployeeID,
                        FailureReason = ex.GetUserFacingMessage()
                    });
                throw;
            }
        }

        private async Task<bool> IsDepartmentExemptFromHoursRulesAsync(Guid departmentId)
        {
            if (departmentId == Guid.Empty)
                return false;

            var departmentConfigs = await _departmentConfigurationRepository
                .GetAllAsync(x => x.DepartmentID == departmentId)
                .ConfigureAwait(false);

            return departmentConfigs.FirstOrDefault()?.EnableExemptRequestsJobsFromHoursRules == true;
        }

        private async Task<bool> IsEmployeeCancellationAllowedAsync()
        {
            var departmentId = _tokenParser.GetDepartmentID();
            if (!departmentId.HasValue || departmentId.Value == Guid.Empty)
                return false;

            var departmentConfigs = await _departmentConfigurationRepository
                .GetAllAsync(x => x.DepartmentID == departmentId.Value)
                .ConfigureAwait(false);

            var departmentConfig = departmentConfigs.FirstOrDefault();
            return departmentConfig?.AllowEmployeeCancellation == true;
        }

        private async Task SendCancellationEmailsAsync(
            PostPosition existingPostPosition,
            string? normalizedReason,
            CancellationToken cancellationToken)
        {
            var assignedEmployee = await _employeeRepository.GetByIdAsync(existingPostPosition.EmployeeID, "EmployeeID", e => e.EmployeeType);

            var recipientEmail = assignedEmployee?.OffDutyCallBackEmail?.Trim() ?? assignedEmployee?.PrimaryEmail?.Trim();

            if (string.IsNullOrEmpty(recipientEmail))
            {
                var employeeDto = _mapper.Map<EmployeeDto>(assignedEmployee);
                await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                    employeeDto,
                    EmailType.CancelledEmail,
                    cancellationToken).ConfigureAwait(false);
            }

            var shiftStartTime = existingPostPosition.StartTime
                ?? existingPostPosition.Post?.PostStartTime
                ?? TimeOnly.MinValue;
            var shiftStartDateTime = existingPostPosition.DateOfWork?.ToDateTime(shiftStartTime)
                ?? GetCurrentDepartmentDateTime();
            var isLateCancellation = !IsTimeGreaterThan72Hours(shiftStartDateTime);

            var postTitle = existingPostPosition.Post?.PostTitle?.Trim() ?? string.Empty;
            var postAssignedDate = existingPostPosition.DateOfWork?.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var postStartTime = existingPostPosition.StartTime?.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;

            // Who cancelled (from token)
            var cancelledByName = string.Empty;
            var cancelledByRank = string.Empty;
            var cancelledByEmployeeType = string.Empty;
            var cancellerEmployeeId = _tokenParser.GetEmployeeID();
            if (cancellerEmployeeId.HasValue && cancellerEmployeeId.Value != Guid.Empty)
            {
                var canceller = await _employeeRepository.GetByIdAsync(cancellerEmployeeId.Value, "EmployeeID", e => e.EmployeeType);
                if (canceller != null)
                {
                    cancelledByName = canceller.FullName;
                    var rankPart = canceller.EmployeeType?.Rank ??
                                   (canceller.EmployeeTypeID != Guid.Empty
                                       ? (await _employeeTypeService.GetByIdAsync(canceller.EmployeeTypeID))?.Rank
                                       : null);
                    cancelledByRank = (rankPart.HasValue && rankPart.Value != 0) ? rankPart.Value.ToString() : string.Empty;
                    cancelledByEmployeeType =
                        canceller.EmployeeType?.EmployeeTypeLong?.Trim() ??
                        canceller.EmployeeType?.EmployeeTypeShort?.Trim() ??
                        string.Empty;
                }
            }

            // 1) Employee cancellation email (existing behavior)
            if (!string.IsNullOrEmpty(recipientEmail) && isLateCancellation)
            {
                var templateParameters = BuildCancellationTemplateParameters(
                    postAssignedDate,
                    postStartTime,
                    postTitle,
                    cancelledByName,
                    cancelledByRank,
                    cancelledByEmployeeType,
                    normalizedReason);

                var sendMailRequest = new SendMailRequestDto(
                    EmailType.CancelledEmail,
                    recipientEmail,
                    fromEmail: null,
                    templateParameters);

                await _emailTemplateService.SendMailAsync(sendMailRequest, cancellationToken);
            }

            // 2) Supervisor/parent cancellation email (latest requested rules)
            if (existingPostPosition.Post?.IsSupervisorPost == true)
            {
                try
                {
                    var postWithRequest = await _postService.GetPostWithRequestAsync(existingPostPosition.Post.PostID);
                    var isSpecialEvent = postWithRequest?.Request?.IsSpecialEvent == true;

                    var supervisorEmail = existingPostPosition.Post.PrimaryEmail?.Trim();
                    var isParent = existingPostPosition.Post.IsParent;
                    var hasParentId = existingPostPosition.Post.PostParentID.HasValue &&
                                      existingPostPosition.Post.PostParentID.Value != Guid.Empty;

                    string? parentEmail = null;
                    // 1) isSupervisor=true AND isParent=true AND isSpecialEvent=true => supervisor only
                    // 2) isSupervisor=true AND isParent=false AND parentId exists AND isSpecialEvent=false => supervisor only
                    // 3) isSpecialEvent=true AND isParent=false AND parentId exists => supervisor + parent
                    if (isSpecialEvent && !isParent && hasParentId)
                    {
                        var parentPost = await _postService.GetByIdAsync(existingPostPosition.Post.PostParentID!.Value);
                        parentEmail = parentPost?.PrimaryEmail?.Trim();
                    }

                    var toEmails = new List<string>();
                    if (!string.IsNullOrWhiteSpace(supervisorEmail))
                        toEmails.Add(supervisorEmail);
                    if (!string.IsNullOrWhiteSpace(parentEmail))
                        toEmails.Add(parentEmail);

                    toEmails = toEmails
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    if (toEmails.Count > 0)
                    {
                        var supervisorTemplateParameters = BuildCancellationTemplateParameters(
                            postAssignedDate,
                            postStartTime,
                            postTitle,
                            cancelledByName,
                            cancelledByRank,
                            cancelledByEmployeeType,
                            normalizedReason);

                        var supervisorMailRequest = new SendMailRequestDto(
                            EmailType.CancelledEmail,
                            toEmails,
                            fromEmail: null,
                            supervisorTemplateParameters);

                        await _emailTemplateService.SendMailAsync(supervisorMailRequest, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    // Best-effort: cancellation should succeed even if notification email fails.
                    _logger.LogWarning(ex, "Failed to send supervisor/parent cancellation email for PostPositionID {PostPositionId}", existingPostPosition.PostPositionID);
                }
            }
        }

        private static Dictionary<string, string> BuildCancellationTemplateParameters(
            string postAssignedDate,
            string postStartTime,
            string postTitle,
            string cancelledByName,
            string cancelledByRank,
            string cancelledByEmployeeType,
            string? cancellationReason)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.CancellationDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.CancellationTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                [EnumHelper.ToStringValue(EmailKeyword.CancelledBy)] = cancelledByName,
                [EnumHelper.ToStringValue(EmailKeyword.CancelledByRank)] = cancelledByRank,
                [EnumHelper.ToStringValue(EmailKeyword.CancelledByEmployeeType)] = cancelledByEmployeeType,
                [EnumHelper.ToStringValue(EmailKeyword.CancellationReason)] = cancellationReason ?? string.Empty
            };
        }

        /// <summary>
        /// If waiting-list rows exist for the same post/date, tries to assign one waiting employee at a time
        /// using RequestService.AssignPostToMe. On conflict (409), falls back to the next waiting employee.
        /// When an assignment succeeds, marks that waiting row as Applied (order 0) and finalizes the source row as Cancelled.
        /// </summary>
        private async Task AssignCancelledPostFromWaitingListAsync(
            PostPosition cancelledPostPosition,
            bool isApproveCancellation,
            CancellationToken cancellationToken = default)
        {
            if (cancelledPostPosition == null || !cancelledPostPosition.DateOfWork.HasValue)
                return;

            var waitingRows = await _waitingPostPositionService
                .GetWaitingListEmployee(cancelledPostPosition.PostID, cancelledPostPosition.DateOfWork.Value)
                .ConfigureAwait(false);

            var orderedWaitingRows = (waitingRows ?? new List<WaitingPostPositionResponseDto>())
                .Where(w => w.WaitingPostPositionID.HasValue && w.EmployeeID != Guid.Empty)
                .OrderBy(w => w.OrderNumber)
                .ThenBy(w => w.CreatedDate ?? DateTime.MinValue)
                .ThenBy(w => w.WaitingPostPositionID)
                .ToList();

            if (orderedWaitingRows.Count == 0)
                return;

            var waitingListAssignSucceeded = false;

            foreach (var waitingRow in orderedWaitingRows)
            {
                var assignResult = await _requestService.AssignPostToMe(
                    cancelledPostPosition.PostID,
                    cancelledPostPosition.DateOfWork.Value,
                    waitingRow.EmployeeID,
                    confirmConflict: false,
                    isBulkAssign: true,
                    postPositionId: cancelledPostPosition.PostPositionID,
                    waitingPostPositionPayRate: waitingRow.WaitingPostPositionPayRate).ConfigureAwait(false);

                // Conflict and other assignment failures should not block the next waiting employee.
                // CheckAvailablePost finalizes the source row to Cancelled during each attempt; keep it
                // there between attempts so the next employee is not blocked by CancellationPending dialog logic.
                if (assignResult?.Status != 200)
                    continue;

                waitingListAssignSucceeded = true;

                await _waitingPostPositionService
                    .MarkWaitingRowAssignedAsync(waitingRow.WaitingPostPositionID!.Value, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (!isApproveCancellation)
                {
                    await UpdateStatusByPostIdAsync(
                            cancelledPostPosition.PostPositionID,
                            PostPositionStatus.Cancelled)
                        .ConfigureAwait(false);
                }
                return;
            }

            if (!waitingListAssignSucceeded &&
                !isApproveCancellation &&
                cancelledPostPosition.PostPositionID != Guid.Empty)
            {
                await _repository.UpdatePostPositionStatusDirectAsync(
                        cancelledPostPosition.PostPositionID,
                        PostPositionStatus.CancellationPending).ConfigureAwait(false);
            }
        }

        public async Task<int> UpdateCallSignAsync(Guid postPositionId, string fieldName, string? fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_UPDATE_CALLSIGN_FIELD_NAME_REQUIRED));
            }

            if (string.Equals(fieldName, nameof(PostPosition.PostCallSignID), StringComparison.OrdinalIgnoreCase))
                return await UpdatePostPositionCallSignAsync(postPositionId, fieldValue).ConfigureAwait(false);

            if (string.Equals(fieldName, nameof(PostPosition.CallSign), StringComparison.OrdinalIgnoreCase))
            {
                var updated = await UpdateAsync(
                    filter: x => x.PostPositionID == postPositionId,
                    updateAction: entity =>
                    {
                        entity.CallSign = string.IsNullOrWhiteSpace(fieldValue) ? null : fieldValue.Trim();
                        entity.PostCallSignID = null;
                    }
                ).ConfigureAwait(false);

                if (updated <= 0)
                {
                    throw new WebApiException(
                        HttpStatusCode.NotFound,
                        "404",
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));
                }

                return updated;
            }

            if (!_updateCallSignAllowedProperties.TryGetValue(fieldName, out var property))
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    "400",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_UPDATE_CALLSIGN_INVALID_FIELD_NAME, fieldName));
            }

            var updatedCount = await UpdateAsync(
                filter: x => x.PostPositionID == postPositionId,
                updateAction: entity => property.SetValue(entity, fieldValue)
            ).ConfigureAwait(false);

            if (updatedCount <= 0)
            {
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    "404",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));
            }

            return updatedCount;
        }

        public async Task SwapCallSignAsync(Guid postPositionId1, Guid postPositionId2)
        {
            if (postPositionId1 == postPositionId2)
                return;

            var swapped = await _repository.SwapCallSignAsync(postPositionId1, postPositionId2).ConfigureAwait(false);
            if (!swapped)
            {
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    "404",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId1));
            }
        }

        private async Task<int> UpdatePostPositionCallSignAsync(Guid postPositionId, string? postCallSignIdValue)
        {
            var postPosition = await _repository.GetByIdAsync(postPositionId, "PostPositionID").ConfigureAwait(false)
                ?? throw new WebApiException(
                    HttpStatusCode.NotFound,
                    "404",
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_NOT_FOUND_BY_ID, postPositionId));

            Guid? newPostCallSignId = null;
            string? newCallSign = null;

            if (!string.IsNullOrWhiteSpace(postCallSignIdValue) &&
                Guid.TryParse(postCallSignIdValue, out var postCallSignId))
            {
                var postCallSign = await _postCallSignService.GetByIdAsync(postCallSignId).ConfigureAwait(false);
                if (postCallSign?.PostID != postPosition.PostID)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        "400",
                        ModelValidationErrorCodes.GetErrorMessage(
                            ModelValidationErrorCodes.JOB_POSITION_UPDATE_CALLSIGN_INVALID_FIELD_NAME,
                            nameof(PostPosition.PostCallSignID)));
                }

                newPostCallSignId = postCallSign.PostCallSignID;
                newCallSign = postCallSign.CallSign;
            }
            else if (!string.IsNullOrWhiteSpace(postCallSignIdValue))
            {
                newPostCallSignId = null;
                newCallSign = postCallSignIdValue.Trim();
            }

            if (postPosition.PostCallSignID == newPostCallSignId &&
                string.Equals(postPosition.CallSign, newCallSign, StringComparison.Ordinal))
                return 1;

            return await UpdateAsync(
                filter: x => x.PostPositionID == postPositionId,
                updateAction: entity =>
                {
                    entity.CallSign = newCallSign;
                    entity.PostCallSignID = newPostCallSignId;
                }).ConfigureAwait(false);
        }

        private static PropertyInfo GetWritableStringProperty(string propertyName)
        {
            var property = typeof(PostPosition).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                throw new InvalidOperationException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_PROPERTY_NOT_FOUND, propertyName, nameof(PostPosition)));
            }
            if (!property.CanWrite)
            {
                throw new InvalidOperationException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_PROPERTY_NOT_WRITABLE, propertyName, nameof(PostPosition)));
            }
            if (property.PropertyType != typeof(string))
            {
                throw new InvalidOperationException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_POSITION_PROPERTY_NOT_STRING, propertyName, nameof(PostPosition)));
            }
            return property;
        }

        /// <summary>
        /// Gets dashboard counts for the logged-in employee including available posts, scheduled jobs, and closed jobs.
        /// </summary>
        /// <param name="searchModel">Search model for available posts.</param>
        /// <param name="pagination">Pagination parameters.</param>
        /// <returns>Dashboard counts for the logged-in employee.</returns>
        public async Task<EmployeeDashboardCountsDto> GetEmployeeDashboardCountsAsync(SearchModel searchModel, PageParam? pagination = null)
        {
            try
            {
                var employeeId = GetValidatedEmployeeId();

                // 1. Get available posts count (for logged-in employee)
                // Note: This creates a dependency on RequestService, but GetAllAvailablePostRequest
                // contains logic that spans both Request and PostPosition domains.
                // Consider extracting to a shared service in the future if this becomes problematic.
                var availablePosts = await _requestService.GetAllAvailablePostRequest(searchModel, null, isAvailablePost: true);
                var availablePostsCount = (int)(availablePosts?.TotalRecords ?? 0);

                // 2. Get scheduled jobs count (all PostPositions for logged-in employee)
                var scheduledJobs = await GetForLoggedInEmployeeAsync(PostPositionStatus.Assigned, false, pagination);
                var scheduledJobsCount = (int)scheduledJobs.TotalRecords;

                // 3. Get closed jobs count (PostPositions for logged-in employee with Closed status)
                var closedJobs = await GetAllAsync(
                    x => !x.IsPlaceholder && x.EmployeeID == employeeId && x.PostPositionStatus == PostPositionStatus.Closed,
                    includes: null,
                    pageParam: pagination
                );
                var closedJobsCount = closedJobs.Count();

                // 4. Get pending jobs count (jobs user applied for, date has passed, but not closed/cancelled)
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                var pendingJobs = await GetAllAsync(
                    x => !x.IsPlaceholder &&
                         x.EmployeeID == employeeId &&
                         x.DateOfWork < currentDate &&
                         x.PostPositionStatus == PostPositionStatus.Assigned,
                    includes: null,
                    pageParam: pagination
                );
                var pendingJobsCount = pendingJobs.Count();

                return new EmployeeDashboardCountsDto
                {
                    AvailablePosts = availablePostsCount,
                    ScheduledJobs = scheduledJobsCount,
                    ClosedJobs = closedJobsCount,
                    PendingJobs = pendingJobsCount
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PostPositionsByPostResponseDto>
            GetPostPositionsWithEmployeeDetailsAsync(
                Guid postId,
                PageParam? pagination = null,
                IReadOnlyList<DateOnly>? dates = null,
                IReadOnlyCollection<RequestStatus>? requestStatuses = null,
                bool includeCompleted = false,
                bool isViewHistory = true)
        {
            var effectivePageParam = pagination ?? new PageParam();
            var all = await GetPostPositionsWithEmployeeAsync(
                postId,
                dateOfWork: null,
                pagination: null,
                datesOfWork: null,
                requestStatuses: requestStatuses,
                includeCompleted: includeCompleted);

            // Same date list as POST by-post/filter: distinct work dates before the selected-date and history filters.
            var workDates = all.Select(x => x.DateOfWork).ExtractWorkDates();

            if (dates is { Count: > 0 })
            {
                var allowed = dates.ToHashSet();
                all = all
                    .Where(x => x.DateOfWork.HasValue && allowed.Contains(x.DateOfWork.Value))
                    .ToList();
            }

            all = isViewHistory
                ? all.Where(x => x.PostPositionStatus == PostPositionStatus.Cancelled).ToList()
                : all.Where(x => x.PostPositionStatus != PostPositionStatus.Cancelled).ToList();

            var page = all
                .OrderBy(x => x.DateOfWork ?? DateOnly.MaxValue)
                .ThenBy(x => x.CallSign ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.LastName ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                .Skip(effectivePageParam.Offset ?? 0)
                .Take(effectivePageParam.Limit ?? PageParam.MinLimit)
                .ToList();

            return new PostPositionsByPostResponseDto(page, effectivePageParam, all.Count, workDates);
        }

        /// <inheritdoc />
        public async Task<List<DateOnly>> GetCollectDatesForPostAsync(
            Guid postId,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            bool includeCompleted = false,
            CancellationToken cancellationToken = default)
        {
            var positions = await GetPostPositionsWithEmployeeAsync(postId, dateOfWork: null, pagination: null, datesOfWork: null, requestStatuses: requestStatuses, includeCompleted: includeCompleted).ConfigureAwait(false);

            return positions
                .Where(p => p.DateOfWork.HasValue)
                .Select(p => p.DateOfWork!.Value)
                .Where(d =>
                    (!startDate.HasValue || d >= startDate.Value) &&
                    (!endDate.HasValue || d <= endDate.Value))
                .Distinct()
                .OrderBy(d => d)
                .ToList();
        }

        /// <inheritdoc />
        public async Task<PagedList<PostPositionInRequestListDto>> GetPostPositionsByPostFilterAsync(
            RequestPostPositionsFilterDto filter,
            bool isViewHistory = true,
            CancellationToken cancellationToken = default)
        {
            var pageParam = filter.PageParam ?? new PageParam();
            var departmentId = _tokenParser.GetDepartmentID().Value;
            var post = await _postService.GetPostWithRequestAsync(filter.PostId).ConfigureAwait(false);
            if (post?.Request == null || post.Request.DepartmentID != departmentId)
                return new RequestPostPositionsResponseDto(pageParam);

            var rows = await _repository.GetByPostFilterAsync(
                filter.PostId,
                filter.StartDate,
                filter.EndDate,
                pagination: null,
                cancellationToken).ConfigureAwait(false);

            var filtered = PostPositionInRequestListDto.FilterByDates(
                rows.Data,
                filter.Dates,
                filter.StartDate,
                filter.EndDate);

            filtered = isViewHistory
                ? filtered.Where(x => x.Status == PostPositionStatus.Cancelled).ToList()
                : filtered.Where(x => x.Status != PostPositionStatus.Cancelled).ToList();

            var totalRecords = filtered.Count;
            var paged = filtered
                .Skip(pageParam.Offset ?? 0)
                .Take(pageParam.Limit ?? PageParam.MinLimit)
                .ToList();

            return new RequestPostPositionsResponseDto(
                paged,
                pageParam,
                totalRecords,
                PostPositionInRequestListDto.ExtractDates(rows.Data));
        }

        /// <inheritdoc />
        public async Task<(List<PostPositionResponseDto> PostPositions, List<WaitingPostPositionResponseDto> WaitingPostPositions)>
            GetAssignmentsAndWaitingList(
            Guid postId,
            DateOnly? startDate = null,
            int? numberOfDays = null)
        {
            var postPositions = new List<PostPositionResponseDto>();
            var waiting = new List<WaitingPostPositionResponseDto>();

            // Apply date filtering whenever startDate is supplied.
            // If numberOfDays is omitted, default to a 15-day window.
            if (!startDate.HasValue)
            {
                postPositions = await GetPostPositionsWithEmployeeAsync(postId, null).ConfigureAwait(false);
                var waitingRows = (await _waitingPostPositionService.GetAllAsync(
                    x => x.PostID == postId && x.PostPositionStatus == PostPositionStatus.Waiting,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)).ToList();

                var waitingDates = waitingRows
                    .Select(x => x.DateOfWork)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                foreach (var date in waitingDates)
                {
                    waiting.AddRange(await GetWaitingListEmployees(postId, date).ConfigureAwait(false));
                }
            }
            else
            {
                if (numberOfDays.HasValue && numberOfDays.Value <= 0)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.NUMBER_OF_DAYS_MUST_BE_GREATER_THAN_ZERO));
                }

                var effectiveNumberOfDays = numberOfDays ?? 15;
                var endDate = DateHelper.GetEndDate(startDate.Value, effectiveNumberOfDays);
                var dates = new List<DateOnly>();
                for (var day = startDate.Value.DayNumber; day <= endDate.DayNumber; day++)
                {
                    dates.Add(DateOnly.FromDayNumber(day));
                }

                foreach (var date in dates)
                {
                    postPositions.AddRange(await GetPostPositionsWithEmployeeAsync(postId, date).ConfigureAwait(false));
                    waiting.AddRange(await GetWaitingListEmployees(postId, date).ConfigureAwait(false));
                }
            }

            postPositions = postPositions
                .OrderBy(x => x.DateOfWork ?? DateOnly.MaxValue)
                .ThenBy(x => x.PostStartTime ?? TimeOnly.MaxValue)
                .ThenBy(x => x.PostPositionID ?? Guid.Empty)
                .ToList();

            waiting = waiting
                .OrderBy(x => x.DateOfWork)
                .ThenBy(x => x.OrderNumber)
                .ThenBy(x => x.WaitingPostPositionID ?? Guid.Empty)
                .ToList();

            return (postPositions, waiting);
        }

        public Task<List<PostPositionResponseDto>> GetPostPositionsWithEmployeeAsync(
            Guid postId,
            DateOnly? dateOfWork,
            PageParam? pagination = null,
            IReadOnlyCollection<DateOnly>? datesOfWork = null,
            IReadOnlyCollection<RequestStatus>? requestStatuses = null,
            bool includeCompleted = false)
        {
            return _repository.GetPostPositionsWithEmployeeColumnsAsync(
                postId,
                dateOfWork,
                datesOfWork,
                requestStatuses,
                includeCompleted,
                pagination);
        }

        private async Task<List<WaitingPostPositionResponseDto>> GetWaitingListEmployees(
            Guid postId,
            DateOnly dateOfWork)
        {
            return await _waitingPostPositionService
                .GetWaitingListEmployee(postId, dateOfWork)
                .ConfigureAwait(false);
        }


        /// <summary>
        /// Helper to fetch post positions for a post (optionally for a specific DateOfWork),
        /// including Employee and EmployeeType navigation properties, without status exclusions.
        /// </summary>
        public async Task<List<PostPosition>> GetPostPositionsWithEmployeeAllStatusesAsync(
            Guid postId,
            DateOnly? dateOfWork,
            PageParam? pagination)
        {
            return await GetPostPositionsWithEmployeeCoreAsync(
                postId,
                dateOfWork,
                pagination,
                excludeNonActiveStatuses: false);
        }

        /// <summary>
        /// Shared query for loading post positions with employee includes.
        /// </summary>
        private async Task<List<PostPosition>> GetPostPositionsWithEmployeeCoreAsync(
            Guid postId,
            DateOnly? dateOfWork,
            PageParam? pagination,
            bool excludeNonActiveStatuses)
        {
            Expression<Func<PostPosition, bool>> filter = x =>
                x.PostID == postId &&
                (!dateOfWork.HasValue || x.DateOfWork == dateOfWork) &&
                (!excludeNonActiveStatuses ||
                    (x.PostPositionStatus != PostPositionStatus.Closed &&
                     x.PostPositionStatus != PostPositionStatus.ClosedAndSplit &&
                     x.PostPositionStatus != PostPositionStatus.Cancelled &&
                     x.PostPositionStatus != PostPositionStatus.Transferred));

            var entities = await _repository.GetAllAsync(
                filter,
                new Expression<Func<PostPosition, object>>[]
                {
                    x => x.Employee,
                    x => x.Employee.EmployeeType,
                    x => x.Placeholder
                },
                pagination);

            return entities.ToList();
        }

        /// <inheritdoc />
        public async Task<PostTypeResponseDto?> GetPostPositionWithPostDetailsByIdAsync(Guid postPositionId)
        {
            var entity = await GetPostPositionWithPostByIdAsync(postPositionId);
            if (entity == null)
                return null;

            var dto = _mapper.Map<PostTypeResponseDto>(entity);

            // Default flags
            dto.ShowVehicle = false;
            dto.ShowEquipment = false;

            // We can only compute highest-rank logic when DateOfWork is known.
            if (!entity.DateOfWork.HasValue)
                return dto;

            // Determine the current employee from token; if not available, keep defaults.
            var currentEmployeeId = _tokenParser.GetEmployeeID();
            if (!currentEmployeeId.HasValue || currentEmployeeId.Value == Guid.Empty)
                return dto;

            // Get all applied post positions for the same post and same DateOfWork,
            // including Employee and EmployeeType so we can determine rank.
            var sameDayPositions = await GetPostPositionsWithEmployeeAllStatusesAsync(
                entity.PostID,
                entity.DateOfWork,
                pagination: null);

            if (!sameDayPositions.Any())
                return dto;
            var postPositions = _mapper.Map<List<PostPositionDto>>(sameDayPositions);
            await ApplyVehicleEquipmentVisibilityAsync(new List<PostTypeResponseDto> { dto },postPositions,currentEmployeeId);
            return dto;
        }

        public async Task<Guid?> GetHighestRankEmployeeIdAsync(IEnumerable<PostPositionDto> positions)
        {
            var list = positions?.ToList() ?? new List<PostPositionDto>();
            if (list.Count == 0)
                return null;

            var tenures = await LoadTenuresForPositionsAsync(list).ConfigureAwait(false);
            var hireDates = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(tenures);
            return EmployeeHighestRankHelper.SelectHighestRankEmployeeId(list, hireDates, tenures);
        }

        public async Task<bool> IsHighestRankEmployeeAsync(Guid? employeeId, IEnumerable<PostPositionDto> positions)
        {
            if (!employeeId.HasValue || employeeId == Guid.Empty)
                return false;

            var list = positions?.ToList() ?? new List<PostPositionDto>();
            if (list.Count == 0)
                return false;

            var tenures = await LoadTenuresForPositionsAsync(list).ConfigureAwait(false);
            var hireDates = EmployeeHighestRankHelper.BuildHireDateMapFromTenureEntities(tenures);
            return EmployeeHighestRankHelper.IsAmongHighestRankEmployees(employeeId.Value, list, hireDates, tenures);
        }

        private async Task<List<EmployeeTenure>> LoadTenuresForPositionsAsync(List<PostPositionDto> positions)
        {
            var employeeIds = positions
                .Select(p => p.EmployeeID)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (employeeIds.Count == 0)
                return new List<EmployeeTenure>();

            return await _viewDetailsEquipmentRepository
                .GetTenuresForEmployeesAsync(employeeIds)
                .ConfigureAwait(false);
        }
        /// <summary>
        /// Gets the employee ID of the splitter (who created the closed post position for the parent) or null.
        /// </summary>
        private async Task<Guid?> GetSplitterEmployeeIdAsync(Guid parentPostPositionId)
        {
            var closed = await _closedPostPositionRepository.GetAllAsync(
                c => c.PostPositionID == parentPostPositionId,
                null,
                null);
            var first = closed?.FirstOrDefault();
            return first?.CreatedUserID;
        }

        /// <summary>
        /// True when the split assignee is the same employee who performed the split (self-split).
        /// </summary>
        private static bool IsSelfSplit(Guid splitterId, Guid splitAssigneeEmployeeId)
        {
            return splitterId != Guid.Empty
                && splitAssigneeEmployeeId != Guid.Empty
                && splitterId == splitAssigneeEmployeeId;
        }

        /// <summary>
        /// Sends "Post Assignment Accepted" email to the splitter when the split employee accepts the post.
        /// </summary>
        private async Task SendSplitAcceptedEmailToSplitterAsync(PostPositionDto postPosition)
        {
            if (postPosition?.ParentPostPositionId == null || postPosition.ParentPostPositionId == Guid.Empty)
                return;
            var splitterId = await GetSplitterEmployeeIdAsync(postPosition.ParentPostPositionId.Value);
            if (!splitterId.HasValue || splitterId.Value == Guid.Empty)
                return;
            if (IsSelfSplit(splitterId.Value, postPosition.EmployeeID))
                return;
            var splitter = await _employeeRepository.GetByIdAsync(splitterId.Value, "EmployeeID");
            var recipientEmail = splitter?.OffDutyCallBackEmail?.Trim() ?? splitter?.PrimaryEmail?.Trim();
            if (string.IsNullOrEmpty(recipientEmail))
            {
                var splitterDto = _mapper.Map<EmployeeDto>(splitter);
                await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                    splitterDto,
                    EmailType.PostAssignmentAcceptedEmail,
                    CancellationToken.None).ConfigureAwait(false);
            }
            if (string.IsNullOrEmpty(recipientEmail))
                return;
            var post = await _postService.GetByIdAsync(postPosition.PostID);
            var employee = await _employeeRepository.GetByIdAsync(postPosition.EmployeeID, "EmployeeID", e => e.EmployeeType);
            if (post == null || employee == null)
                return;
            var now = DateTime.UtcNow;
            var postTitle = post.PostTitle?.Trim() ?? string.Empty;
            var postAssignedDate = postPosition.DateOfWork?.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var postStartTime = (postPosition.PostStartTime ?? post.PostStartTime).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            var acceptedDate = now.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var acceptedTime = now.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            var employeeName = employee.FullName;
            var employeeType = employee.EmployeeType?.EmployeeTypeLong?.Trim() ?? employee.EmployeeType?.EmployeeTypeShort?.Trim() ?? string.Empty;
            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.AcceptedDate)] = acceptedDate,
                [EnumHelper.ToStringValue(EmailKeyword.AcceptedTime)] = acceptedTime,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = employeeName,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeType)] = employeeType
            };
            var sendMailRequest = new SendMailRequestDto(EmailType.PostAssignmentAcceptedEmail, new List<string> { recipientEmail }, fromEmail: null, templateParameters);
            await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);
        }

        /// <summary>
        /// Sends "Post Assignment Rejected" email to the splitter when the split employee declines the post.
        /// </summary>
        private async Task SendSplitRejectedEmailToSplitterAsync(PostPositionDto postPosition)
        {
            if (postPosition?.ParentPostPositionId == null || postPosition.ParentPostPositionId == Guid.Empty)
                return;
            var splitterId = await GetSplitterEmployeeIdAsync(postPosition.ParentPostPositionId.Value);
            if (!splitterId.HasValue || splitterId.Value == Guid.Empty)
                return;
            if (IsSelfSplit(splitterId.Value, postPosition.EmployeeID))
                return;
            var splitter = await _employeeRepository.GetByIdAsync(splitterId.Value, "EmployeeID");
            var recipientEmail = splitter?.OffDutyCallBackEmail?.Trim() ?? splitter?.PrimaryEmail?.Trim();
            if (string.IsNullOrEmpty(recipientEmail))
            {
                var splitterDto = _mapper.Map<EmployeeDto>(splitter);
                await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                    splitterDto,
                    EmailType.PostAssignmentRejectedEmail,
                    CancellationToken.None).ConfigureAwait(false);
            }
            if (string.IsNullOrEmpty(recipientEmail))
                return;
            var post = await _postService.GetByIdAsync(postPosition.PostID);
            var employee = await _employeeRepository.GetByIdAsync(postPosition.EmployeeID, "EmployeeID", e => e.EmployeeType);
            if (post == null || employee == null)
                return;
            var now = DateTime.UtcNow;
            var postTitle = post.PostTitle?.Trim() ?? string.Empty;
            var postAssignedDate = postPosition.DateOfWork?.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var postStartTime = (postPosition.PostStartTime ?? post.PostStartTime).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            var rejectedDate = now.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var rejectedTime = now.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            var employeeName = employee.FullName;
            var employeeType = employee.EmployeeType?.EmployeeTypeLong?.Trim() ?? employee.EmployeeType?.EmployeeTypeShort?.Trim() ?? string.Empty;
            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                [EnumHelper.ToStringValue(EmailKeyword.RejectedDate)] = rejectedDate,
                [EnumHelper.ToStringValue(EmailKeyword.RejectedTime)] = rejectedTime,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = employeeName,
                [EnumHelper.ToStringValue(EmailKeyword.EmployeeType)] = employeeType
            };
            var sendMailRequest = new SendMailRequestDto(EmailType.PostAssignmentRejectedEmail, new List<string> { recipientEmail }, fromEmail: null, templateParameters);
            await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);
        }

        /// <summary>
        /// Gets a post position entity by ID with the Post navigation property included.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <returns>The post position entity with Post included, or null if not found.</returns>
        private async Task<PostPosition?> GetPostPositionWithPostByIdAsync(Guid postPositionId)
        {
            return await _repository.GetByIdAsync(postPositionId, "PostPositionID", x => x.Post, x => x.Employee);
        }


        /// <summary>
        /// Filters dates to exclude past dates and today if current time is past post start time.
        /// Centralizes the "today logic" used across the service.
        /// </summary>
        /// <param name="dates">The dates to filter.</param>
        /// <param name="postStartTime">The post start time (optional).</param>
        /// <returns>Filtered list of dates that are valid for assignment (future dates and today if time hasn't passed).</returns>
        private List<DateOnly> FilterValidFutureDates(IEnumerable<DateOnly> dates, TimeOnly? postStartTime = null)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            return dates
                .Where(date => date >= today) // Exclude past dates
                .Where(date =>
                {
                    // If date is today, check if current time is past post start time
                    if (date == today)
                    {
                        // If post has a start time and current time is past it, exclude today
                        if (postStartTime.HasValue && currentTime >= postStartTime.Value)
                        {
                            return false; // Exclude today
                        }
                    }
                    return true; // Include future dates and today (if time hasn't passed)
                })
                .ToList();
        }

        /// <summary>
        /// Gets available dates for a post by checking position availability per date.
        /// This method centralizes the logic used by GetAllAvailablePostRequest to ensure consistency.
        /// Only returns dates that have available positions (not fully booked).
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <param name="request">The request DTO.</param>
        /// <param name="post">The post DTO.</param>
        /// <returns>List of dates that have available positions for this post.</returns>
        private async Task<List<DateOnly>> GetAvailableDatesForPostAsync(Guid postId, RequestDto request, PostDto post)
        {
            // Get valid dates for the request using request's StartDate and EndDate
            var validDates = request.CollectDates(request.StartDate, request.EndDate);

            if (!validDates.Any())
                return new List<DateOnly>();

            // Filter dates: exclude past dates and today if current time is past post start time
            var filteredValidDates = FilterValidFutureDates(validDates, post.PostStartTime);

            if (!filteredValidDates.Any())
                return new List<DateOnly>();

            // Get existing PostPositions for this post to check availability per date
            var existingPostPositionsForPost = (await GetAllAsync(
                filter: pp => pp.PostID == postId &&
                             pp.DateOfWork.HasValue &&
                             filteredValidDates.Contains(pp.DateOfWork.Value) &&
                             pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                             pp.PostPositionStatus != PostPositionStatus.Transferred,
                includes: null
            )).ToList();

            // Calculate available positions per date and return only dates with available positions
            var availableDates = new List<DateOnly>();
            foreach (var workDate in filteredValidDates)
            {
                var existingCountForDate = existingPostPositionsForPost
                    .Count(pp => pp.DateOfWork == workDate);
                var availablePositionsCount = post.TotalPostPositions - existingCountForDate;

                if (availablePositionsCount > 0)
                {
                    availableDates.Add(workDate);
                }
            }

            return availableDates;
        }
        /// <inheritdoc />
        public async Task<Guid?> CallSignBulkAssignSourcePostPositionIdAsync(Guid postId, DateOnly dateOfWork)
        {
            var cancelledRows = (await GetAllAsync(
                filter: x =>
                    x.PostID == postId &&
                    x.DateOfWork == dateOfWork &&
                    (x.PostPositionStatus == PostPositionStatus.Cancelled),
                includes: null,
                pageParam: null).ConfigureAwait(false)).ToList();

            if (cancelledRows.Count == 0)
                return null;

            static DateTime SortKey(PostPositionDto row) =>
                row.EditedDate ?? row.CreatedDate ?? DateTime.MinValue;

            return cancelledRows
                .Where(x => x.PostPositionID.HasValue && x.PostPositionID.Value != Guid.Empty).OrderByDescending(SortKey)
                .Select(x => x.PostPositionID!.Value)
                .FirstOrDefault();
        }

        public async Task<List<PostPositionDto>> GetAssignedPostPositionsForRequestAsync(Guid requestId)
        {
            return (await GetAllAsync(
                pp => pp.Post != null
                      && pp.Post.RequestID == requestId
                      && pp.DateOfWork.HasValue
                      && pp.EmployeeID != Guid.Empty
                      && (pp.PostPositionStatus == PostPositionStatus.Assigned
                          || pp.PostPositionStatus == PostPositionStatus.CancellationPending),
                includes: new Expression<Func<PostPosition, object>>[]
                {
                    pp => pp.Post!
                },
                pageParam: null).ConfigureAwait(false)).ToList();
        }

        public async Task UpdateFuturePostPositionPayRatesForRequestAsync(Guid requestId)
        {
            var departmentNow = GetCurrentDepartmentDateTime();
            var assignedPositions = await GetAssignedPostPositionsForRequestAsync(requestId).ConfigureAwait(false);

            var updatesByRate = assignedPositions
                .Where(pp =>
                    pp.PostPositionID.HasValue
                    && pp.Post != null
                    && !pp.IsPostPositionDateTimePassed(departmentNow)
                    && (pp.PostPositionPayRate != pp.Post.PostPayRate
                        || pp.IsPremiumPaidEvent != pp.Post.IsPremiumPaidEvent))
                .GroupBy(pp => new { pp.Post!.PostPayRate, pp.Post.IsPremiumPaidEvent })
                .Select(group => new
                {
                    PayRate = group.Key.PostPayRate,
                    IsPremiumPaidEvent = group.Key.IsPremiumPaidEvent,
                    PostPositionIds = group
                        .Select(pp => pp.PostPositionID!.Value)
                        .ToList()
                })
                .ToList();

            foreach (var update in updatesByRate)
            {
                await UpdateAsync(
                    pp => update.PostPositionIds.Contains(pp.PostPositionID),
                    pp =>
                    {
                        pp.PostPositionPayRate = update.PayRate;
                        pp.IsPremiumPaidEvent = update.IsPremiumPaidEvent;
                    })
                    .ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public async Task UpdateFuturePostPositionExemptFlagForPostAsync(Guid postId, bool isExemptHours)
        {
            if (postId == Guid.Empty)
                return;

            var post = await _postService.GetPostWithRequestAsync(postId).ConfigureAwait(false);
            if (post?.Request == null)
                return;

            var isExemptPosition = isExemptHours
                && await IsDepartmentExemptFromHoursRulesAsync(post.Request.DepartmentID).ConfigureAwait(false);

            var departmentNow = GetCurrentDepartmentDateTime();
            var assignedPositions = (await GetAllAsync(
                    pp => pp.PostID == postId
                          && pp.DateOfWork.HasValue
                          && pp.EmployeeID != Guid.Empty
                          && (pp.PostPositionStatus == PostPositionStatus.Assigned
                              || pp.PostPositionStatus == PostPositionStatus.CancellationPending),
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false)).ToList();

            var postPositionIds = assignedPositions
                .Where(pp =>
                    pp.PostPositionID.HasValue
                    && !pp.IsPostPositionDateTimePassed(departmentNow)
                    && pp.IsExemptPosition != isExemptPosition)
                .Select(pp => pp.PostPositionID!.Value)
                .ToList();

            if (postPositionIds.Count == 0)
                return;

            await UpdateAsync(
                    pp => postPositionIds.Contains(pp.PostPositionID),
                    pp => pp.IsExemptPosition = isExemptPosition)
                .ConfigureAwait(false);
        }

        public async Task<PostPositionDto> AssignToPostMeAsync(Guid postId, Guid employeeId, DateOnly dateOfWork, bool isPlaceHolder = false, PlaceholderDto? placeholder = null, Guid? sourcePostPositionId = null, bool enableInvoiceAndPayroll = false, decimal? waitingPostPositionPayRate = null)
        {
            var post = await _postService.GetPostWithRequestAsync(postId).ConfigureAwait(false);
            var sourcePostPosition = sourcePostPositionId.HasValue && sourcePostPositionId.Value != Guid.Empty
                ? await GetByIdAsync(sourcePostPositionId.Value)
                : null;

            Guid? postCallSignId = null;
            string? callSign = null;
            if (sourcePostPosition != null)
            {
                postCallSignId = sourcePostPosition.PostCallSignID;
                callSign = sourcePostPosition.CallSign;
            }
            else
            {
                var nextCallSign = await _postCallSignService.GetNextAvailableAsync(postId, dateOfWork).ConfigureAwait(false);
                if (nextCallSign != null)
                {
                    postCallSignId = nextCallSign.PostCallSignID;
                    callSign = nextCallSign.CallSign;
                }
            }

            var dto = new PostPositionDto
            {
                PostID = postId,
                EmployeeID = employeeId,
                DateOfWork = dateOfWork,
                PostStartTime = post.PostStartTime,
                PostEndTime = post.PostEndTime,
                LengthOfWork = post.PostLength,
                PostPositionStatus = PostPositionStatus.Assigned,
                CallSign = callSign,
                PostCallSignID = postCallSignId,
                IsPlaceholder = isPlaceHolder,
                PlaceholderDetail = placeholder,
                EnableInvoiceAndPayroll = enableInvoiceAndPayroll,
                PostPositionPayRate = waitingPostPositionPayRate ?? post.PostPayRate,
                IsPremiumPaidEvent = post.IsPremiumPaidEvent,
                IsExemptPosition = post.IsExemptHours
                    && post.Request != null
                    && await IsDepartmentExemptFromHoursRulesAsync(post.Request.DepartmentID).ConfigureAwait(false),
                IsAllocationFinalized = sourcePostPosition?.IsAllocationFinalized == true
            };
            return await UpsertAsync(dto);
        }

        /// <summary>
        /// Checks if an employee has any existing post positions on a specific date.
        /// Used to prevent date conflicts when assigning employees to posts.
        /// </summary>
        /// <param name="employeeId">The employee ID to check.</param>
        /// <param name="dateOfWork">The date to check for conflicts.</param>
        /// <returns>True if employee has a non-cancelled post position on the date; otherwise, false.</returns>
        public async Task<bool> HasEmployeePostPositionOnDateAsync(Guid employeeId, DateOnly dateOfWork)
        {
            var existingPostPositionsForEmployeeOnDate = await GetAllAsync(
                filter: pp => pp.EmployeeID == employeeId &&
                             pp.DateOfWork == dateOfWork &&
                             pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                             pp.PostPositionStatus != PostPositionStatus.Transferred,
                includes: null
            );

            return existingPostPositionsForEmployeeOnDate.Any();
        }

        public async Task<List<PostPositionDto>> GetEmployeesPostPositionsByEmployeeIdAndDateRangeAsync(List<Guid> employeeIds,DateOnly rangeStart,DateOnly rangeEnd,Guid? departmentId = null)
        {
            var allPosts = new List<PostPosition>();

            foreach (var empId in employeeIds)
            {
                var posts = await _repository.GetPostPositionByEmployeeAndDateRangeAsync(empId,rangeStart,rangeEnd,departmentId,excludeCancelledOnly: false).ConfigureAwait(false);

                if (posts != null && posts.Count > 0)
                    allPosts.AddRange(posts);
            }

            return _mapper.Map<List<PostPositionDto>>(allPosts);
        }
        public async Task<EmployeePolicyUsageDto> GetEmployeeStatistics(DepartmentPolicyDto? departmentPolicy, Guid? employeeId = null, DateOnly? date = null, DateTime? dateTime = null)
        {
            if (departmentPolicy == null)
                return new EmployeePolicyUsageDto();
            employeeId ??= _tokenParser.GetEmployeeID();

            if (date == null || dateTime == null)
            {
                (date, dateTime) = GetDateAsPerDepartmentTimeZone(DateOnly.FromDateTime(DateTime.UtcNow));
            }
            var today = date.Value;
            var departmentTimeZone = GetDepartmentTimeZone();
            var departmentToday = GetCurrentDepartmentDate(); // DateOnly ✅
            // 🔹 Department week range for TODAY
            var (weekStart, weekEnd) = today.GetDepartmentWeekRange(departmentPolicy.OnDutyStartDayOfWeek.ToDayOfWeek());

            // 🔹 Expand range (prev + current + next month)
            var rangeStart = new DateOnly(today.Year, today.Month, 1).AddMonths(-1);
            var rangeEnd = new DateOnly(today.Year, today.Month, 1).AddMonths(2).AddDays(-1);

            var postPositionsDto = await GetPostPositionByEmployeeAndDateRangeAsync(employeeId.Value,rangeStart,rangeEnd,null,excludeCancelledOnly: false,enableExemptRequestsJobsFromHoursRules: departmentPolicy.EnableExemptRequestsJobsFromHoursRules).ConfigureAwait(false);
            /* =========================
               PER-DAY MAPS
               ========================= */
            var postsPerDay = postPositionsDto.CountPostsPerDay();
            var hoursPerDay = postPositionsDto.CalculateHoursPerDay();
            var openPostsPerDay = postPositionsDto.CountPostsPerDay(p => p.Post?.ReleaseAs == ReleaseAs.Open);
            var tenurePostsPerDay = postPositionsDto.CountPostsPerDay(p => p.Post?.ReleaseAs == ReleaseAs.TenureBased);
            var managedPostsPerDay = postPositionsDto.CountPostsPerDay(p => p.Post?.ReleaseAs == ReleaseAs.Managed);

            var weekStartDay = departmentPolicy.OnDutyStartDayOfWeek.ToDayOfWeek();
            var openPostsPerWeek = openPostsPerDay.ToWeeklyCountMap(weekStartDay);
            var tenurePostsPerWeek = tenurePostsPerDay.ToWeeklyCountMap(weekStartDay);
            var managedPostsPerWeek = managedPostsPerDay.ToWeeklyCountMap(weekStartDay);
            /* =========================
               PER-WEEK MAPS (IMPORTANT)
               ========================= */
            var postsPerWeek = postsPerDay.ToWeeklyCountMap(weekStartDay);
            var hoursPerWeek = hoursPerDay.ToWeeklyHoursMap(weekStartDay);
            var overduePostPositions = new List<PostPositionOverdueDto>();
            var now = GetCurrentDepartmentDateTime();

            var notClosedPostPositions = postPositionsDto.Where(p =>
                p.PostPositionStatus != PostPositionStatus.Closed &&
                p.PostPositionStatus != PostPositionStatus.Transferred &&
                p.PostPositionStatus != PostPositionStatus.ClosedAndSplit &&
                p.PostPositionID.HasValue &&
                p.DateOfWork.HasValue &&
                p.PostStartTime.HasValue &&
                p.GetPostPositionEndDateTime().HasValue);
            foreach (var post in notClosedPostPositions)
            {
                var endDateTime = post.GetPostPositionEndDateTime()!.Value;

                // Post not finished yet → skip
                if (now <= endDateTime)
                    continue;

                var overdueHours = (decimal)(now - endDateTime).TotalHours;

                overduePostPositions.Add(new PostPositionOverdueDto
                {
                    PostPositionID = post.PostPositionID!.Value,
                    DateOfWork = post.DateOfWork.Value,
                    OverdueHours = overdueHours
                });
            }
            // =========================
            // APPLIED POSTS COUNT (BY CREATED DATE)
            // =========================

            var appliedPostsTodayCount = postPositionsDto
                .Where(p =>
                    p.PostPositionID.HasValue &&
                    p.CreatedDate.HasValue && // ✅ NULL CHECK
                    DateOnly.FromDateTime(
                        TimeZoneInfo.ConvertTimeFromUtc(p.CreatedDate.Value, departmentTimeZone)
                    ) == departmentToday
                )
                .Count();
            return new EmployeePolicyUsageDto
            {
                EmployeeId = employeeId.Value,
                Date = today,
                DateTime = dateTime.Value,
                PostsPerDay = postsPerDay,
                HoursPerDay = hoursPerDay,
                PostsPerWeek = postsPerWeek,
                HoursPerWeek = hoursPerWeek,
                OpenPostsPerDay = openPostsPerDay,
                OpenPostsPerWeek = openPostsPerWeek,
                TenurePostsPerDay = tenurePostsPerDay,
                TenurePostsPerWeek = tenurePostsPerWeek,
                ManagedPostsPerDay = managedPostsPerDay,
                ManagedPostsPerWeek = managedPostsPerWeek,
                OverduePostPositions = overduePostPositions,
                AppliedPostsToday = appliedPostsTodayCount
            };
        }

        
        /// <summary>
        /// Sends assignment email for a post position using the post position ID.
        /// Retrieves post position details and calls the modified SendAssignPostEmailAsync method.
        /// </summary>
        /// <param name="postPositionId">The post position ID to send email for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendAssignPostEmailByPostPositionIdAsync(Guid postPositionId, CancellationToken cancellationToken = default)
        {
            if (postPositionId == Guid.Empty)
                throw new ArgumentException("Post position ID is required.", nameof(postPositionId));

            // Get the post position with related data
            var postPosition = await GetByIdAsync(postPositionId);
            if (postPosition == null)
                throw new KeyNotFoundException($"Post position with ID {postPositionId} not found.");

            // Get the post details
            var post = await _postService.GetByIdAsync(postPosition.PostID);
            if (post == null)
                throw new KeyNotFoundException($"Post with ID {postPosition.PostID} not found.");

            // Get employee ID from token
            var employeeId = GetValidatedEmployeeId();
            if (!employeeId.HasValue || employeeId.Value == Guid.Empty)
                throw new UnauthorizedAccessException("Employee ID not found in token.");

            // Call the modified SendAssignPostEmailAsync method
            await _requestService.SendAssignPostEmailAsync(
                employeeId: employeeId.Value,
                email: null, // Let the method resolve the email from the employee
                postTitle: post.PostTitle?.Trim() ?? string.Empty,
                dateOfWork: postPosition.DateOfWork,
                startTime: postPosition.PostStartTime ?? post.PostStartTime,
                postLength: postPosition.LengthOfWork > 0 ? postPosition.LengthOfWork : post.PostLength,
                assignedEmployeeName: null,
                assignedEmployeeType: null,
                assignedEmployeeRank: null,
                pushNotificationTitle: null,
                pushNotificationBody: null,
                departmentId: _tokenParser.GetDepartmentID(),
                cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task<List<PostPosition>> GetAssignedRealEmployeesByRequestAndDateAsync(
            Guid requestId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default) =>
            _repository.GetAssignedRealEmployeesByRequestAndDateAsync(requestId, dateOfWork, cancellationToken);

        /// <inheritdoc />
        public Task<OneTimeDateChangeReviewDataDto> GetOneTimeDateChangeReviewDataAsync(
            Guid requestId,
            DateOnly oldDate,
            DateOnly newDate,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default) =>
            _repository.GetOneTimeDateChangeReviewDataAsync(requestId, oldDate, newDate, departmentId, cancellationToken);

        /// <inheritdoc />
        public async Task<PagedList<EmployeePostListingItemDto>> GetTodaysJobsAsync(
            PageParam? pagination = null,
            CancellationToken cancellationToken = default)
        {
            var pageParam = pagination ?? new PageParam();
            var employeeId = _tokenParser.GetEmployeeID() ?? Guid.Empty;

            if (employeeId == Guid.Empty)
                return new PagedList<EmployeePostListingItemDto>(Array.Empty<EmployeePostListingItemDto>(), pageParam, 0);

            var rows = await _repository.GetTodaysJobsAsync(
                employeeId,
                GetCurrentDepartmentDate(),
                _tokenParser.GetDepartmentID(),
                cancellationToken).ConfigureAwait(false);

            var totalRecords = rows.Count;
            var page = pagination == null
                ? rows
                : rows.Skip(pageParam.Offset ?? 0).Take(pageParam.Limit ?? PageParam.MinLimit);

            rows = page.Select(row =>
            {
                row.Title = row.Title?.Trim() ?? string.Empty;
                row.Location = row.Location?.Trim() ?? string.Empty;
                if (row.PostEndTime == null && row.StartTime.HasValue && row.PostLength > 0)
                    row.PostEndTime = row.StartTime.Value.AddMinutes(row.PostLength.Value);
                return row;
            }).ToList();

            return new PagedList<EmployeePostListingItemDto>(rows, pageParam, totalRecords);
        }
    }
}