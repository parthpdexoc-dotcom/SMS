using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using SMS.Data;
using SMS.Generic.Common;
using SMS.Generic.Common.Constants;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Repositories.Repositories;
using SMS.Generic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing ClosedPostPositionEquipment operations using the generic service pattern.
    /// </summary>
    public class ClosedPostPositionEquipmentService : GenericService<ClosedPostPositionEquipmentDto, ClosedPostPositionEquipment>, IClosedPostPositionEquipmentService
    {
        private readonly IDepartmentEquipmentService _departmentEquipmentService;
        private readonly IPostService _postService;
        private readonly IClosedPostPositionService _closedPostPositionService;
        private readonly IPostPositionService _postPositionService;
        private readonly IPostPositionRepository _postPositionRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IRequestService _requestService;
        private readonly IRequesterService _requesterService;
        private readonly IResponseService _responseService;
        private readonly IAnswersService _answersService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ITokenParser? _tokenParser;
        private readonly IFirebasePushNotificationService _pushNotificationService;
        private readonly IInvoiceDetailService _invoiceDetailService;
        private readonly IInvoiceService _invoiceService;
        private readonly IPostWorkflowAuditService _postWorkflowAuditService;
        private readonly IPostPositionFlowAuditService _postPositionFlowAuditService;
        private readonly IActivityFormService _activityFormService;
        private readonly IPostPositionVehicleService _postPositionVehicleService;
        private readonly SMSDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionEquipmentService"/> class.
        /// </summary>
        /// <param name="repository">The closed post position equipment repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="departmentEquipmentService">The department equipment service for FK validation.</param>
        /// <param name="postService">The post service for FK validation.</param>
        /// <param name="closedPostPositionService">The closed post position service for FK and ownership validation.</param>
        /// <param name="tokenParser">The token parser for authentication.</param>
        public ClosedPostPositionEquipmentService(
            IClosedPostPositionEquipmentRepository repository,
            IMapper mapper,
            IDepartmentEquipmentService departmentEquipmentService,
            IPostService postService,
            IClosedPostPositionService closedPostPositionService,
            IPostPositionService postPositionService,
            IPostPositionRepository postPositionRepository,
            IEmployeeService employeeService,
            IRequestService requestService,
            IRequesterService requesterService,
            IResponseService responseService,
            IAnswersService answersService,
            IEmailTemplateService emailTemplateService,
            IFirebasePushNotificationService pushNotificationService,
            IInvoiceDetailService invoiceDetailService,
            IInvoiceService invoiceService,
            IPostWorkflowAuditService postWorkflowAuditService,
            IPostPositionFlowAuditService postPositionFlowAuditService,
            IActivityFormService activityFormService,
            IPostPositionVehicleService postPositionVehicleService,
            SMSDbContext dbContext,
            ITokenParser? tokenParser = null)
            : base(repository, mapper, "ClosedPostPositionEquipmentID", tokenParser)
        {
            _departmentEquipmentService = departmentEquipmentService;
            _postService = postService;
            _closedPostPositionService = closedPostPositionService;
            _postPositionService = postPositionService;
            _postPositionRepository = postPositionRepository;
            _employeeService = employeeService;
            _requestService = requestService;
            _requesterService = requesterService;
            _responseService = responseService;
            _answersService = answersService;
            _emailTemplateService = emailTemplateService;
            _tokenParser = tokenParser;
            _pushNotificationService = pushNotificationService ?? throw new ArgumentNullException(nameof(pushNotificationService));
            _invoiceDetailService = invoiceDetailService ?? throw new ArgumentNullException(nameof(invoiceDetailService));
            _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
            _postWorkflowAuditService = postWorkflowAuditService ?? throw new ArgumentNullException(nameof(postWorkflowAuditService));
            _postPositionFlowAuditService = postPositionFlowAuditService ?? throw new ArgumentNullException(nameof(postPositionFlowAuditService));
            _activityFormService = activityFormService ?? throw new ArgumentNullException(nameof(activityFormService));
            _postPositionVehicleService = postPositionVehicleService ?? throw new ArgumentNullException(nameof(postPositionVehicleService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Gets the closed post position and its related equipment records for a given post position ID.
        /// </summary>
        /// <param name="postPositionId">The post position ID.</param>
        /// <returns>The closed post position with its equipment records; otherwise null if not found.</returns>
        public async Task<ClosedPostPositionDetailsDto?> GetClosedDetailsByPostPositionIdAsync(Guid postPositionId)
        {
            var closedItems = await _closedPostPositionService.GetAllAsync(
                x => x.PostPositionID == postPositionId,
                null,
                null);

            var closed = closedItems?
                .OrderByDescending(x => x.IsEdited == true)
                .ThenByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                .FirstOrDefault();
            if (closed == null || !closed.ClosedPostPositionID.HasValue)
                return null;

            return await BuildClosedDetailsAsync(postPositionId, closed);
        }

        public async Task<List<ClosedPostPositionDetailsDto>> GetClosedDetailsListByPostPositionIdAsync(Guid postPositionId)
        {
            var closedItems = (await _closedPostPositionService.GetAllAsync(
                x => x.PostPositionID == postPositionId,
                null,
                null))
                .OrderByDescending(x => x.IsEdited == true)
                .ThenByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                .ToList();

            if (!closedItems.Any())
                return new List<ClosedPostPositionDetailsDto>();

            var detailsList = new List<ClosedPostPositionDetailsDto>();
            foreach (var closed in closedItems)
            {
                if (!closed.ClosedPostPositionID.HasValue || closed.ClosedPostPositionID.Value == Guid.Empty)
                    continue;

                detailsList.Add(await BuildClosedDetailsAsync(postPositionId, closed));
            }

            return detailsList;
        }

        private async Task<ClosedPostPositionDetailsDto> BuildClosedDetailsAsync(Guid postPositionId, ClosedPostPositionDto closed)
        {
            var equipments = await GetAllAsync(
                x => x.ClosedPostPositionID == closed.ClosedPostPositionID,
                null,
                null);

            // Enrich equipments with DepartmentEquipment info (name + IsVehicle) without N+1 calls
            var equipmentList = equipments?.ToList() ?? new List<ClosedPostPositionEquipmentDto>();
            var departmentEquipmentIds = equipmentList
                .Where(e => e.DepartmentEquipmentID.HasValue && e.DepartmentEquipmentID.Value != Guid.Empty)
                .Select(e => e.DepartmentEquipmentID!.Value)
                .Distinct()
                .ToList();

            var deptEquipDict = new Dictionary<Guid, DepartmentEquipmentDto>();
            if (departmentEquipmentIds.Count > 0)
            {
                Expression<Func<DepartmentEquipment, object>>[] includes = { x => x.EquipmentType };

                var deptEquips = await _departmentEquipmentService.GetAllAsync(
                    filter: de => departmentEquipmentIds.Contains(de.DepartmentEquipmentID),
                    includes: includes,
                    pageParam: null);

                deptEquipDict = deptEquips
                    .Where(de => de.DepartmentEquipmentID.HasValue)
                    .GroupBy(de => de.DepartmentEquipmentID!.Value)
                    .ToDictionary(g => g.Key, g => g.First());
            }

            var enriched = equipmentList
                .Select(e =>
                {
                    if (e.DepartmentEquipmentID.HasValue
                        && deptEquipDict.TryGetValue(e.DepartmentEquipmentID.Value, out var de)
                        && de != null)
                    {
                        e.DepartmentEquipmentName =
                            de.EquipmentType?.EquipementTypeShort
                            ?? de.EquipmentType?.EquipementTypeLong;
                        e.DepartmentEquipmentIsVehicle = de.EquipmentType?.IsVehicle;
                    }

                    return e;
                })
                .ToList();

            // Load questionnaire submissions (responses + answers) for this post position.
            var submissions = new List<QuestionnaireSubmissionDto>();
            var submissionInternalData = new Dictionary<Guid, List<SMS.Generic.Dtos.Dtos.AnswerItemInternal>>();
            var responses = await _responseService.GetAllAsync(
                filter: r => r.PostPositionId == postPositionId,
                includes: null,
                pageParam: null);

            var responseList = responses?.ToList() ?? new List<ResponseDto>();
            foreach (var response in responseList)
            {
                if (!response.QuestionnaireId.HasValue || response.QuestionnaireId.Value == Guid.Empty)
                    continue;
                if (!response.ResponseId.HasValue || response.ResponseId.Value == Guid.Empty)
                    continue;

                var answers = await _answersService.GetAllAsync(
                    filter: a => a.ResponseId == response.ResponseId,
                    includes: null,
                    pageParam: null);

                // Internal class to hold answer data during processing
                var answerItems = (answers ?? Enumerable.Empty<AnswersDto>())
                    .Where(a => a.QuestionId.HasValue && a.QuestionId.Value != Guid.Empty)
                    .Select(a => new AnswerItemInternal
                    {
                        QuestionId = a.QuestionId,
                        OriginalAnswer = a.Answer,
                        SelectedOptionIds = ParseOptionIdsFromJson(a.Answer)
                    })
                    .ToList();

                static List<Guid>? ParseOptionIdsFromJson(string? answer)
                {
                    if (string.IsNullOrWhiteSpace(answer)) return null;
                    var trimmed = answer.Trim();

                    // Handle single plain GUID
                    if (Guid.TryParse(trimmed, out var singleGuid) && singleGuid != Guid.Empty)
                        return new List<Guid> { singleGuid };

                    // Handle JSON array of GUIDs
                    if (trimmed.Length < 2 || trimmed[0] != '[') return null;
                    try
                    {
                        var strings = JsonSerializer.Deserialize<List<string>>(answer);
                        if (strings == null || strings.Count == 0) return null;
                        var list = new List<Guid>();
                        foreach (var s in strings)
                            if (Guid.TryParse(s, out var g) && g != Guid.Empty)
                                list.Add(g);
                        return list.Count > 0 ? list : null;
                    }
                    catch (JsonException)
                    {
                        return null;
                    }
                }

                submissions.Add(new QuestionnaireSubmissionDto
                {
                    QuestionnaireId = response.QuestionnaireId,
                    Answers = answerItems.Select(a => new QuestionnaireAnswerItemDto
                    {
                        QuestionId = a.QuestionId,
                        Answer = a.OriginalAnswer // Will be replaced with option text during enrichment
                    }).ToList()
                });

                // Store internal data for enrichment
                submissionInternalData[response.QuestionnaireId.Value] = answerItems;
            }

            await _activityFormService.EnrichQuestionnaireSubmissionsAsync(submissions, submissionInternalData).ConfigureAwait(false);

            return new ClosedPostPositionDetailsDto
            {
                ClosedPostPosition = closed,
                Equipments = enriched,
                QuestionnaireSubmissions = submissions
            };
        }

        /// <summary>
        /// Inserts a new closed post position equipment record or updates an existing one.
        /// </summary>
        /// <param name="dto">The closed post position equipment DTO to insert or update.</param>
        /// <returns>The upserted closed post position equipment DTO.</returns>
        public override async Task<ClosedPostPositionEquipmentDto> UpsertAsync(ClosedPostPositionEquipmentDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            // Validate ClosedPostPositionID exists and load for ownership/PostID checks
            if (!dto.ClosedPostPositionID.HasValue)
            {
                var message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_ID);
                if (string.IsNullOrWhiteSpace(message))
                    message = ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_ID;
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), message);
            }

            await ValidateForeignKeyAsync<IClosedPostPositionService, ClosedPostPositionDto>(
                _closedPostPositionService,
                dto.ClosedPostPositionID.Value,
                "ClosedPostPosition");

            var closedPostPosition = await _closedPostPositionService.GetByIdAsync(dto.ClosedPostPositionID.Value);
            if (closedPostPosition == null)
            {
                var message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_CLOSED_JOB_POSITION_ID);
                if (string.IsNullOrWhiteSpace(message))
                    message = "ClosedPostPosition not found.";
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), message);
            }

            //Duplicate - same ClosedPostPositionID + DepartmentEquipmentID
            if (dto.DepartmentEquipmentID.HasValue)
            {
                await ValidateCompositeKeyDuplicateAsync(
                    dto: dto,
                    filterExpression: e => e.ClosedPostPositionID == dto.ClosedPostPositionID && e.DepartmentEquipmentID == dto.DepartmentEquipmentID,
                    errorMessage: ModelValidationErrorCodes.DUPLICATE_CLOSED_JOB_POSITION_EQUIPMENT);
            }

            // PostID must match parent ClosedPostPosition when provided
            if (dto.PostID.HasValue && closedPostPosition.PostID != dto.PostID.Value)
            {
                var message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_EQUIPMENT_JOB_ID_MISMATCH);
                if (string.IsNullOrWhiteSpace(message))
                    message = ModelValidationErrorCodes.CLOSED_JOB_POSITION_EQUIPMENT_JOB_ID_MISMATCH;
                throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), message);
            }

            // Validate DepartmentEquipmentID exists
            if (dto.DepartmentEquipmentID.HasValue)
            {
                await ValidateForeignKeyAsync<IDepartmentEquipmentService, DepartmentEquipmentDto>(
                    _departmentEquipmentService,
                    dto.DepartmentEquipmentID.Value,
                    "DepartmentEquipment");
            }

            // Validate PostID exists when provided
            if (dto.PostID.HasValue)
            {
                await ValidateForeignKeyAsync<IPostService, PostDto>(
                    _postService,
                    dto.PostID.Value,
                    "Post");
            }

            await ApplyReimburseToFromAssignedEmployeeAsync(dto, closedPostPosition);

            return await base.UpsertAsync(dto);
        }

        /// <summary>
        /// Sets <see cref="ClosedPostPositionEquipmentDto.ReimburseTo"/> from the assigned post-position employee's
        /// <see cref="EmployeeDto.IsSubsidizedVehicle"/> flag (subsidized → Employee, otherwise City).
        /// </summary>
        private async Task ApplyReimburseToFromAssignedEmployeeAsync(
            ClosedPostPositionEquipmentDto dto,
            ClosedPostPositionDto closedPostPosition)
        {
            if (!closedPostPosition.PostPositionID.HasValue || closedPostPosition.PostPositionID.Value == Guid.Empty)
            {
                dto.ReimburseTo = ReimburseTo.City;
                return;
            }

            var postPosition = await _postPositionService.GetByIdAsync(closedPostPosition.PostPositionID.Value);
            if (postPosition == null
                || postPosition.IsPlaceholder
                || postPosition.EmployeeID == Guid.Empty)
            {
                dto.ReimburseTo = ReimburseTo.City;
                return;
            }

            var employee = await _employeeService.GetByIdAsync(postPosition.EmployeeID);
            dto.ReimburseTo = employee?.IsSubsidizedVehicle == true
                ? ReimburseTo.Employee
                : ReimburseTo.City;
        }

        /// <summary>
        /// Splits and closes a post position, creating closed records and updating post position status.
        /// </summary>
        public async Task<PostPositionDto> SplitAndCloseAsync(SplitPostPositionRequestDto request)
        {
            try
            {
                await ValidateSplitAllowedForSuspendedSkillAsync(request);

                if (request != null)
                    request.IsSplitRequest ??= true;

                return await HandleCloseAsync(
                    request: request,
                    status: PostPositionStatus.ClosedAndSplit,
                    createSplitPostPosition: true,
                    markIsSplit: true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ValidateSplitAllowedForSuspendedSkillAsync(SplitPostPositionRequestDto? request)
        {
            var SplitEmployeeID = request?.SplitEmployeeID;
            if (!SplitEmployeeID.HasValue || SplitEmployeeID.Value == Guid.Empty)
                return;

            var effectiveDate = request?.DateOfWork ?? request?.WorkStartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

            var employee = await _employeeService.GetByIdAsync(SplitEmployeeID.Value);
            if (employee == null)
                return;

            var hasActiveSuspendedSkill = employee.IsSuspendedOn(effectiveDate);
            if (hasActiveSuspendedSkill)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.CLOSED_JOB_POSITION_SPLIT_NOT_ALLOWED_SUSPENDED_EMPLOYEE);
            }
        }

        /// <summary>
        /// Closes a post position, creating closed records and updating post position status.
        /// </summary>
        public async Task<PostPositionDto> CloseAsync(SplitPostPositionRequestDto request)
        {
            try
            {
                if (request != null)
                    request.IsSplitRequest ??= false;

                return await HandleCloseAsync(
                    request: request,
                    status: PostPositionStatus.Closed,
                    createSplitPostPosition: false,
                    markIsSplit: false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UncloseAsync(Guid postPositionId)
        {
            if (postPositionId == Guid.Empty)
                throw new ArgumentException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_JOB_POSITION_ID)
                    ?? ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_JOB_POSITION_ID,
                    nameof(postPositionId));

            var existingPostPosition = await _postPositionService.GetByIdAsync(postPositionId);
            if (existingPostPosition == null)
                throw new WebApiException(
                    HttpStatusCode.NotFound,
                    ((int)HttpStatusCode.NotFound).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND)
                    ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND);

            if (existingPostPosition.PostPositionStatus == PostPositionStatus.Unclosed)
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_ALREADY_UNCLOSED)
                    ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_ALREADY_UNCLOSED);

            var relatedClosedPosts = await _closedPostPositionService.GetAllAsync(
                x => x.PostPositionID == postPositionId,
                null,
                null);

            var targetClosedPost = relatedClosedPosts
                .Where(x =>
                    x.ClosedPostPositionID.HasValue &&
                    x.ClosedPostPositionID.Value != Guid.Empty &&
                    x.Status == ClosedPostPositionStatus.Closed)
                .OrderByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                .ThenByDescending(x => x.ClosedPostPositionID)
                .FirstOrDefault();

            if (targetClosedPost == null || !targetClosedPost.ClosedPostPositionID.HasValue || targetClosedPost.ClosedPostPositionID.Value == Guid.Empty)
            {
                throw new WebApiException(
                    HttpStatusCode.BadRequest,
                    ((int)HttpStatusCode.BadRequest).ToString(),
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_ALREADY_UNCLOSED)
                    ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_ALREADY_UNCLOSED);
            }

            var closedPostPositionId = targetClosedPost.ClosedPostPositionID.Value;
            await _invoiceDetailService.EnsureUncloseInvoiceAllowedAsync(closedPostPositionId);

            var updatedCount = await _postPositionService.UpdateStatusByPostIdAsync(postPositionId, PostPositionStatus.Unclosed);

            if (updatedCount > 0)
            {
                await _closedPostPositionService.UpdateClosedPostPositionStatusAsync(closedPostPositionId, ClosedPostPositionStatus.Unclosed);

                // When unclosing, insert a reversing (Debit) invoice detail line (if invoiced) and recalc invoice totals.
                await _invoiceDetailService.ApplyDebitForUncloseAsync(closedPostPositionId);
            }

            if (updatedCount > 0 && !existingPostPosition.IsPlaceholder && existingPostPosition.EmployeeID != Guid.Empty)
            {
                var employee = await _employeeService.GetByIdAsync(existingPostPosition.EmployeeID);
                var recipientEmail = employee?.OffDutyCallBackEmail?.Trim() ?? employee?.PrimaryEmail?.Trim();

                if (string.IsNullOrEmpty(recipientEmail))
                {
                    await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                        employee,
                        EmailType.AssignPostEmail,
                        CancellationToken.None).ConfigureAwait(false);
                }
                if (!string.IsNullOrEmpty(recipientEmail))
                {
                    var post = await _postService.GetByIdAsync(existingPostPosition.PostID);
                    var postTitle = post?.PostTitle?.Trim() ?? string.Empty;
                    var postAssignedDate = existingPostPosition.DateOfWork?.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture) ?? string.Empty;
                    var postStartTime = (existingPostPosition.PostStartTime ?? post?.PostStartTime)?.ToString("h:mm tt", CultureInfo.InvariantCulture) ?? string.Empty;

                    var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                        [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                        [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime
                    };

                    var sendMailRequest = new SendMailRequestDto(
                        EmailType.AssignPostEmail,
                        recipientEmail,
                        fromEmail: null,
                        templateParameters);

                    await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);
                }
            }

            return updatedCount;
        }

        private async Task<PostPositionDto> HandleCloseAsync(
            SplitPostPositionRequestDto request,
            PostPositionStatus status,
            bool createSplitPostPosition,
            bool markIsSplit)

            {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                if (!request.PostPositionID.HasValue || request.PostPositionID.Value == Guid.Empty)
                    throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_ID_REQUIRED), nameof(request.PostPositionID));

                // Load the post position so we can derive PostID and post times when needed
                var existingPostPosition = await _postPositionService.GetByIdAsync(request.PostPositionID.Value, x => x.Post);
                if (existingPostPosition == null)
                    throw new KeyNotFoundException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND)
                        ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND);

                var hasParentClosedPostPositionId =
                    request.ParentClosedPostPositionID.HasValue &&
                    request.ParentClosedPostPositionID.Value != Guid.Empty;

                // Source closed-post-position row used to add the Credit invoice detail after creating the new close row.
                Guid? creditSourceOldClosedPostPositionId = null;

                // Normal re-close flow (unclose -> close again, no explicit parent from UI):
                // discover the latest unclosed closed-post-position history row for this post position.
                if (!hasParentClosedPostPositionId && existingPostPosition.PostPositionStatus == PostPositionStatus.Unclosed)
                {
                    var priorClosedPosts = await _closedPostPositionService.GetAllAsync(
                        x => x.PostPositionID == request.PostPositionID.Value,
                        null,
                        null);

                    var prior = (priorClosedPosts ?? Enumerable.Empty<ClosedPostPositionDto>())
                        .Where(x =>
                            x.ClosedPostPositionID.HasValue &&
                            x.ClosedPostPositionID.Value != Guid.Empty &&
                            x.Status == ClosedPostPositionStatus.Unclosed)
                        .OrderByDescending(x => x.CreatedDate ?? DateTime.MinValue)
                        .FirstOrDefault();

                    if (prior?.ClosedPostPositionID.HasValue == true)
                        creditSourceOldClosedPostPositionId = prior.ClosedPostPositionID.Value;
                    request.ParentClosedPostPositionID = creditSourceOldClosedPostPositionId;
                    request.IsEdited = true;
                    request.Status = ClosedPostPositionStatus.Closed;
                }
                // First close (no parent row, assignment not in Unclosed): persist Closed on the new closed-post-position row.
                else if (!hasParentClosedPostPositionId && existingPostPosition.PostPositionStatus != PostPositionStatus.Unclosed)
                {
                    request.Status = ClosedPostPositionStatus.Closed;
                }

                hasParentClosedPostPositionId =
                    request.ParentClosedPostPositionID.HasValue &&
                    request.ParentClosedPostPositionID.Value != Guid.Empty;

                // If PostID is not provided (or empty) on the request, populate it from the post position
                if (!request.PostID.HasValue || request.PostID == Guid.Empty)
                    request.PostID = existingPostPosition.PostID;

                if (existingPostPosition.Post != null)
                {
                    request.PostStartTime = existingPostPosition.Post.PostStartTime;
                    request.PostEndTime = existingPostPosition.Post.PostEndTime;
                }

                // CloseBy is the logged-in employee performing the close (not the assigned officer or placeholder name).
                if (_tokenParser != null)
                {
                    var closerEmployeeId = _tokenParser.GetEmployeeID();
                    if (closerEmployeeId.HasValue && closerEmployeeId.Value != Guid.Empty)
                    {
                        var closer = await _employeeService.GetByIdAsync(closerEmployeeId.Value).ConfigureAwait(false);
                        var closeByName = closer.GetFullName();
                        if (!string.IsNullOrWhiteSpace(closeByName))
                            request.CloseBy = closeByName;
                    }
                }

                if (!request.CloseDateTime.HasValue)
                    request.CloseDateTime = DateTime.UtcNow;

                // Validate split-related fields before any DB writes (so we never persist "closed" without split when split fails)
                if (request.IsSplitRequest == true || createSplitPostPosition)
                {
                    if (!request.DateOfWork.HasValue)
                    {
                        var msg = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_SPLIT_DATE_OF_WORK)
                                  ?? ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_SPLIT_DATE_OF_WORK;
                        throw new ArgumentException(msg);
                    }
                    if (!request.StartTime.HasValue)
                        throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_SPLIT_START_TIME));
                    if (createSplitPostPosition)
                    {
                        if (!request.WorkStartDate.HasValue)
                            throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_WORK_START_DATE_REQUIRED_FOR_SPLIT));
                        if (!request.WorkStartTime.HasValue)
                            throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_WORK_START_TIME_REQUIRED_FOR_SPLIT));
                    }
                }

                PostPositionStatus? splitPrecheckOriginalStatus = null;
                var splitPrecheckOriginalIsSplit = false;
                ClosedPostPositionDto closedPostPosition = null!;
                PostPositionDto result = null!;

                try
                {
                    if (request.IsSplitRequest == true)
                    {
                        if (!request.SplitEmployeeID.HasValue || request.SplitEmployeeID.Value == Guid.Empty)
                            throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_SPLIT_EMPLOYEE_ID));

                        // Same pattern as post transfer: mark the source ClosedAndSplit before availability checks
                        // so CheckAvailablePost sees a free slot on this post/date for the split remainder.

                        splitPrecheckOriginalStatus = existingPostPosition.PostPositionStatus;
                        splitPrecheckOriginalIsSplit = existingPostPosition.IsSplit;

                        await _postPositionRepository.SetClosedAndSplitStatusAsync(
                                request.PostPositionID!.Value,
                                _tokenParser?.GetEmployeeID(),
                                _tokenParser?.GetUsername()).ConfigureAwait(false);
                        // If the app shuts down after this SetClosedAndSplitStatusAsync function and before close/split completes, the row stays
                        // ClosedAndSplit until manually corrected or a later rollback/retry runs.

                        var availablePostResult = await _requestService.CheckAvailablePost(
                            request.PostID.Value,
                            request.DateOfWork.Value,
                            request.SplitEmployeeID.Value, true);
                        if (availablePostResult.HasTimeConflictRequiringConfirmation && !request.ConfirmConflict)
                        {
                            var conflictMessage = !string.IsNullOrWhiteSpace(availablePostResult.Message)
                                ? availablePostResult.Message
                                : (ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_SPLIT_JOB_UNAVAILABLE)
                                   ?? ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_SPLIT_JOB_UNAVAILABLE);
                            throw new WebApiException(HttpStatusCode.Conflict, ((int)HttpStatusCode.Conflict).ToString(), conflictMessage);
                        }
                        if (availablePostResult.HasError && !availablePostResult.HasTimeConflictRequiringConfirmation && IsDuplicateJobAssignmentOnDateAvailabilityError(availablePostResult.Message, request))
                        {
                            var errorMessage = !string.IsNullOrWhiteSpace(availablePostResult.Message)
                                ? availablePostResult.Message
                                : (ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_SPLIT_JOB_UNAVAILABLE)
                                   ?? ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_SPLIT_JOB_UNAVAILABLE);
                            throw new WebApiException(HttpStatusCode.BadRequest, ((int)HttpStatusCode.BadRequest).ToString(), errorMessage);
                        }
                    }

                // Validate questionnaire user before any DB writes
                if (request.QuestionnaireSubmissions != null && request.QuestionnaireSubmissions.Any())
                {
                    var createdUserId = request.CreatedUserID ?? _tokenParser?.GetEmployeeID() ?? request.SplitEmployeeID;
                    if (!createdUserId.HasValue || createdUserId.Value == Guid.Empty)
                        throw new ArgumentException(ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_CREATED_USER_OR_SPLIT_EMPLOYEE_REQUIRED));
                }

                ValidateEquipmentMileageBeforeClose(request);

                if (hasParentClosedPostPositionId)
                {
                    var oldClosedPostPosition = await _closedPostPositionService.GetByIdAsync(request.ParentClosedPostPositionID.Value);
                    if (oldClosedPostPosition == null || !oldClosedPostPosition.ClosedPostPositionID.HasValue || oldClosedPostPosition.ClosedPostPositionID.Value == Guid.Empty)
                    {
                        throw new WebApiException(
                            HttpStatusCode.BadRequest,
                            ((int)HttpStatusCode.BadRequest).ToString(),
                            ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_PARENT_NOT_FOUND)
                            ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_PARENT_NOT_FOUND);
                    }

                    // Edit flow: always create a new history row for each edit.
                    request.IsEdited = true;
                    request.Status = ClosedPostPositionStatus.Closed;
                    creditSourceOldClosedPostPositionId = oldClosedPostPosition.ClosedPostPositionID.Value;
                }
                if (request.EndTime == null && request.RequestedEndTimePayload.HasValue)
                    request.EndTime = request.RequestedEndTimePayload;

                request.ApplyMinimumHoursPaidPolicy(_tokenParser?.GetDepartmentPolicy());

                if (creditSourceOldClosedPostPositionId.HasValue)
                {
                    var creditHours = Math.Abs(request.GetBillableHours());
                    decimal? validateVehicleFee = null;
                    decimal? validateEquipmentFee = null;
                    if (request.EquipmentDetails is { Count: > 0 })
                    {
                        var (vehicleCharge, equipmentCharge) =
                            await CalculateEquipmentChargesAsync(
                                request.EquipmentDetails,
                                request.PostID,
                                request.IsMinimumDue == true ? request.GetBillableHours() : null).ConfigureAwait(false);
                        validateVehicleFee = vehicleCharge;
                        validateEquipmentFee = equipmentCharge;
                        request.TotalVehicleFee = vehicleCharge;
                        request.TotalEquipmentFee = equipmentCharge;
                    }

                    await _invoiceDetailService.ApplyCreditForRecloseAsync(
                        creditSourceOldClosedPostPositionId.Value,
                        Guid.Empty,
                        creditHours,
                        validateVehicleFee,
                        validateEquipmentFee,
                        validateOnly: true).ConfigureAwait(false);
                }

                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                    {
                        await using var transaction = await _dbContext.Database.BeginTransactionAsync().ConfigureAwait(false);
                        try
                        {
                        if (hasParentClosedPostPositionId)
                        {
                            var parentInvalidatedCount = await _closedPostPositionService.UpdateClosedPostPositionStatusAsync(
                                request.ParentClosedPostPositionID!.Value,
                                ClosedPostPositionStatus.Unclosed,
                                isEdited: false);

                            if (parentInvalidatedCount <= 0)
                            {
                                throw new WebApiException(
                                    HttpStatusCode.BadRequest,
                                    ((int)HttpStatusCode.BadRequest).ToString(),
                                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_PARENT_NOT_FOUND)
                                    ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_PARENT_NOT_FOUND);
                            }

                            await _invoiceDetailService.ApplyDebitForUncloseAsync(
                                request.ParentClosedPostPositionID!.Value,
                                deferRecalculation: true);
                        }

                        closedPostPosition = await _closedPostPositionService.UpsertAsync(request);

                        if (request.EquipmentDetails != null && request.EquipmentDetails.Any())
                        {
                            foreach (var equipment in request.EquipmentDetails)
                            {
                                if (!equipment.DepartmentEquipmentID.HasValue)
                                    continue;

                                equipment.ClosedPostPositionID = closedPostPosition.ClosedPostPositionID;
                                equipment.PostID = request.PostID;
                                await UpsertAsync(equipment);
                            }
                        }

                        if (request.QuestionnaireSubmissions != null && request.QuestionnaireSubmissions.Any())
                        {
                            var createdUserId = request.CreatedUserID ?? _tokenParser?.GetEmployeeID() ?? request.SplitEmployeeID;
                            await UpsertQuestionnaireSubmissionsAsync(
                                request.PostPositionID!.Value,
                                createdUserId!.Value,
                                request.QuestionnaireSubmissions);
                        }

                        if (creditSourceOldClosedPostPositionId.HasValue
                            && closedPostPosition?.ClosedPostPositionID.HasValue == true
                            && closedPostPosition.ClosedPostPositionID.Value != Guid.Empty)
                        {
                            var creditHours = Math.Abs(closedPostPosition.GetBillableHours());
                            var (creditVehicleFee, creditEquipmentFee) =
                                await CalculateEquipmentChargesForClosedPostPositionAsync(closedPostPosition.ClosedPostPositionID.Value);

                            await _invoiceDetailService.ApplyCreditForRecloseAsync(
                                creditSourceOldClosedPostPositionId.Value,
                                closedPostPosition.ClosedPostPositionID.Value,
                                creditHours,
                                creditVehicleFee,
                                creditEquipmentFee);
                        }

                        await _postPositionService.UpdateAsync(
                            filter: x => x.PostPositionID == request.PostPositionID!.Value,
                            updateAction: entity =>
                            {
                                entity.PostPositionStatus = status;
                                entity.WasUnclosed = false;
                                if (markIsSplit)
                                    entity.IsSplit = true;
                            });

                        if (createSplitPostPosition)
                        {
                            var newPostPosition = new PostPositionDto(
                                source: existingPostPosition,
                                employeeId: request.SplitEmployeeID!.Value,
                                status: PostPositionStatus.SplitDecisionPending,
                                parentPostPositionId: existingPostPosition.PostPositionID,
                                splitPostPositionId: existingPostPosition.PostPositionID,
                                isSplit: false,
                                dateOfWork: request.DateOfWork!.Value,
                                postStartTime: request.StartTime!.Value,
                                postEndTime: request.EndTime!.Value);
                            newPostPosition.CallSign = existingPostPosition.CallSign;

                            result = await _postPositionService.UpsertAsync(newPostPosition);

                            // Pass source vehicles to the split employee with no rank check.
                            if (result.PostPositionID is Guid splitChildId
                                && splitChildId != Guid.Empty
                                && request.PostPositionID is Guid splitSourceId
                                && splitSourceId != Guid.Empty)
                            {
                                await _postPositionVehicleService
                                    .CopyAllocationsAsync(splitSourceId, splitChildId)
                                    .ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            var updated = await _postPositionService.GetByIdAsync(request.PostPositionID!.Value);
                            if (updated == null)
                                throw new KeyNotFoundException(
                                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND)
                                    ?? ModelValidationErrorCodes.CLOSED_JOB_POSITION_JOB_POSITION_NOT_FOUND);
                            result = updated;
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        throw;
                    }
                  }).ConfigureAwait(false);
                }
                catch
                {
                    await RollbackStatusAsync(
                        request.PostPositionID!.Value,
                        existingPostPosition.PostPositionStatus,
                        existingPostPosition.IsSplit,
                        _tokenParser?.GetEmployeeID(),
                        _tokenParser?.GetUsername()).ConfigureAwait(false);

                    throw;
                }

                if (createSplitPostPosition)
                {
                    // Send split assignment email to the employee with whom the post was split,
                    // and a split confirmation email to the splitter.
                    if (request.PostID.HasValue && request.SplitEmployeeID.HasValue && request.SplitEmployeeID.Value != Guid.Empty)
                    {
                        var post = await _postService.GetByIdAsync(request.PostID.Value);
                        var splitEmployee = await _employeeService.GetByIdAsync(request.SplitEmployeeID.Value, e => e.EmployeeType);
                        var recipientEmail = splitEmployee?.OffDutyCallBackEmail?.Trim() ?? splitEmployee?.PrimaryEmail?.Trim();
                        if (string.IsNullOrEmpty(recipientEmail))
                        {
                            await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                                splitEmployee,
                                EmailType.SplitPostEmail,
                                CancellationToken.None).ConfigureAwait(false);
                        }
                        if (post != null && splitEmployee != null && !string.IsNullOrEmpty(recipientEmail))
                        {
                            var postTitle = post.PostTitle?.Trim() ?? string.Empty;
                            var postAssignedDate = existingPostPosition.DateOfWork?.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture) ?? string.Empty;
                            var startTime = existingPostPosition.PostStartTime ?? post.PostStartTime;
                            var postStartTime = startTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
                            var closeDateTime = request.CloseDateTime ?? DateTime.UtcNow;
                            var splitDate = closeDateTime.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                            var splitTime = closeDateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);

                            var splitByName = string.Empty;
                            var splitByEmployeeId = _tokenParser?.GetEmployeeID();
                            var splitByEmployee = default(EmployeeDto);
                            if (splitByEmployeeId.HasValue && splitByEmployeeId.Value != Guid.Empty)
                            {
                                splitByEmployee = await _employeeService.GetByIdAsync(splitByEmployeeId.Value, e => e.EmployeeType);
                                if (splitByEmployee != null)
                                    splitByName = splitByEmployee.GetFullName();
                            }

                            var splitEmployeeName = splitEmployee.GetFullName();
                            var splitEmployeeType = splitEmployee.EmployeeType?.EmployeeTypeLong?.Trim()
                                ?? splitEmployee.EmployeeType?.EmployeeTypeShort?.Trim() ?? string.Empty;

                            var vehicles = string.Empty;
                            var equipment = string.Empty;
                            if (result.PostPositionID is Guid splitChildId && splitChildId != Guid.Empty)
                            {
                                (vehicles, equipment) = await _postPositionVehicleService
                                    .GetAssignedDisplayNamesAsync(splitChildId)
                                    .ConfigureAwait(false);
                            }

                            // Email 1: to split employee (Assignment Split Notification)
                            var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                            {
                                [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                                [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                                [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                                [EnumHelper.ToStringValue(EmailKeyword.SplitDate)] = splitDate,
                                [EnumHelper.ToStringValue(EmailKeyword.SplitTime)] = splitTime,
                                [EnumHelper.ToStringValue(EmailKeyword.SplitBy)] = splitByName,
                                [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = splitEmployeeName,
                                [EnumHelper.ToStringValue(EmailKeyword.EmployeeType)] = splitEmployeeType,
                                [EnumHelper.ToStringValue(EmailKeyword.Vehicles)] = vehicles,
                                [EnumHelper.ToStringValue(EmailKeyword.Equipment)] = equipment
                            };

                            var sendMailRequest = new SendMailRequestDto(
                                EmailType.SplitPostEmail,
                                new List<string> { recipientEmail },
                                fromEmail: null,
                                templateParameters);
                            await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);

                            // Email 2: to splitter (Post Split Confirmation)
                            if (splitByEmployee != null)
                            {
                                var splitterEmail = splitByEmployee.OffDutyCallBackEmail?.Trim() ?? splitByEmployee.PrimaryEmail?.Trim();
                                if (string.IsNullOrEmpty(splitterEmail))
                                {
                                    await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                                        splitByEmployee,
                                        EmailType.PostSplitConfirmationEmail,
                                        CancellationToken.None).ConfigureAwait(false);
                                }
                                if (!string.IsNullOrEmpty(splitterEmail))
                                {
                                    var splitterTemplateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                    {
                                        [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                                        [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = postAssignedDate,
                                        [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = postStartTime,
                                        [EnumHelper.ToStringValue(EmailKeyword.SplitDate)] = splitDate,
                                        [EnumHelper.ToStringValue(EmailKeyword.SplitTime)] = splitTime,
                                        [EnumHelper.ToStringValue(EmailKeyword.SplitBy)] = splitByName,
                                        [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = splitEmployeeName,
                                        [EnumHelper.ToStringValue(EmailKeyword.EmployeeType)] = splitEmployeeType,
                                        [EnumHelper.ToStringValue(EmailKeyword.Vehicles)] = vehicles,
                                        [EnumHelper.ToStringValue(EmailKeyword.Equipment)] = equipment
                                    };

                                    var splitterMailRequest = new SendMailRequestDto(
                                        EmailType.PostSplitConfirmationEmail,
                                        new List<string> { splitterEmail },
                                        fromEmail: null,
                                        splitterTemplateParameters);
                                    await _emailTemplateService.SendMailAsync(splitterMailRequest, CancellationToken.None);
                                }
                            }
                        }
                    }
                }

                // Send close-post email only when closing the post (not when splitting).
                if (!createSplitPostPosition && request.PostID.HasValue)
                {
                    var recipientEmails = new List<string>();
                    var post = await _postService.GetByIdAsync(request.PostID.Value);
                    if (post != null)
                    {
                        var requestDto = await _requestService.GetByIdAsync(post.RequestID.Value);
                        var requesterId = requestDto?.RequesterID;
                        if (requesterId.HasValue && requesterId.Value != Guid.Empty)
                        {
                            var requester = await _requesterService.GetByIdAsync(requesterId.Value);
                            var requesterEmail = requester?.NotificationRecipients?.Trim();
                            if (!string.IsNullOrEmpty(requesterEmail))
                                recipientEmails.Add(requesterEmail);
                            if (!string.IsNullOrEmpty(requesterEmail))
                            {
                                var postTitle = post.PostTitle?.Trim() ?? string.Empty;
                                var closeDateTime = request.CloseDateTime ?? DateTime.Now;
                                var closePostDate = closeDateTime.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
                                var closePostTime = closeDateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);

                                // The closer is always the logged-in employee (placeholder assignments have no assigned employee).
                                var closerEmployeeId = _tokenParser?.GetEmployeeID() ?? Guid.Empty;

                                var closedByName = string.Empty;
                                var closedByRank = string.Empty;
                                var closedByEmployeeType = string.Empty;
                                var closer = (closerEmployeeId != Guid.Empty)
                                    ? await _employeeService.GetByIdAsync(closerEmployeeId, e => e.EmployeeType)
                                    : null;
                                if (closer != null)
                                {
                                    closedByName = closer.GetFullName();
                                    var rank = closer.EmployeeType?.Rank;
                                    closedByRank = (rank.HasValue && rank.Value != 0) ? rank.Value.ToString() : string.Empty;
                                    closedByEmployeeType = closer.EmployeeType?.EmployeeTypeLong?.Trim()
                                        ?? closer.EmployeeType?.EmployeeTypeShort?.Trim() ?? string.Empty;
                                }

                                var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                                {
                                    [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                                    [EnumHelper.ToStringValue(EmailKeyword.ClosePostDate)] = closePostDate,
                                    [EnumHelper.ToStringValue(EmailKeyword.ClosePostTime)] = closePostTime,
                                    [EnumHelper.ToStringValue(EmailKeyword.ClosedByName)] = closedByName,
                                    [EnumHelper.ToStringValue(EmailKeyword.ClosedByRank)] = closedByRank,
                                    [EnumHelper.ToStringValue(EmailKeyword.ClosedByEmployeeType)] = closedByEmployeeType
                                };

                                var shouldNotifyCloser = !existingPostPosition.IsPlaceholder && closerEmployeeId != Guid.Empty;
                                if (shouldNotifyCloser)
                                {
                                    var closerEmp = await _employeeService.GetByIdAsync(closerEmployeeId);
                                    var closerEmail = closerEmp?.OffDutyCallBackEmail?.Trim() ?? closerEmp?.PrimaryEmail?.Trim();
                                    if (!string.IsNullOrEmpty(closerEmail))
                                        recipientEmails.Add(closerEmail);

                                    if (string.IsNullOrEmpty(closerEmail))
                                    {
                                        await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                                            closerEmp,
                                            EmailType.ClosePostEmail,
                                            CancellationToken.None).ConfigureAwait(false);
                                    }
                                }

                                if (recipientEmails.Count > 0)
                                {
                                    var sendMailRequest = new SendMailRequestDto(
                                        EmailType.ClosePostEmail,
                                        recipientEmails,
                                        fromEmail: null,
                                        templateParameters);
                                    await _emailTemplateService.SendMailAsync(sendMailRequest, CancellationToken.None);
                                }
                                // Best-effort push notification to closer on close.
                                if (shouldNotifyCloser)
                                {
                                    var title = "Post Closed";
                                    var body = $"{postTitle} was closed on {closePostDate} at {closePostTime}.";
                                    await _pushNotificationService.SendToEmployeesAsync(
                                        new[] { closerEmployeeId },
                                        title,
                                        body,
                                        CancellationToken.None);
                                }
                            }
                        }
                    }
                }
                result.Post = null;
                await _postWorkflowAuditService.LogSuccessAsync(
                    status,
                    PostWorkflowAuditContext.FromRequest(request, result),
                    CancellationToken.None).ConfigureAwait(false);

                await _postPositionFlowAuditService.LogCloseFlowAsync(
                    request,
                    existingPostPosition,
                    result,
                    status,
                    createSplitPostPosition,
                    CancellationToken.None).ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                await _postWorkflowAuditService.LogFailureAsync(
                    status,
                    ex,
                    PostWorkflowAuditContext.FromRequest(request, failureReason: ex.GetUserFacingMessage()),
                    CancellationToken.None).ConfigureAwait(false);
                throw;
            }
        }

        /// <summary>
        /// When a close is recorded as <see cref="FinancialImpactType.Credit"/> (re-close after edit),
        /// appends a new invoice detail line on the same invoice as the prior line for the previous closed post position.
        /// Resolves invoice by <see cref="InvoiceDetailDto.ClosePostPositionID"/> matching the old closed row, then falls back to request + job date range.
        /// Line amounts use the same rules as <see cref="RequestService.GetRequestDataForInvoiceAsync"/> via <see cref="InvoiceDetailDto.FromPostPosition"/>.
        /// </summary>
        

        /// <summary>
        /// Upserts questionnaire response and answers for one or more questionnaire submissions.
        /// Called on final close/split submit only (not on "next" step).
        /// </summary>
        /// <param name="postPositionId">The post position being closed/split.</param>
        /// <param name="createdUserId">User submitting (for Response.CreatedUserID).</param>
        /// <param name="submissions">One or more questionnaire submissions (questionnaireId + answers).</param>
        private async Task UpsertQuestionnaireSubmissionsAsync(
            Guid postPositionId,
            Guid createdUserId,
            List<QuestionnaireSubmissionDto> submissions)
        {
            if (submissions == null || !submissions.Any())
                return;

            var now = DateTime.UtcNow;
            var submitterEmail = string.Empty;
            if (_tokenParser != null)
            {
                var tokenEmployeeId = _tokenParser.GetEmployeeID();
                if (tokenEmployeeId.HasValue && tokenEmployeeId.Value != Guid.Empty)
                {
                    var employee = await _employeeService.GetByIdAsync(tokenEmployeeId.Value);
                    submitterEmail = employee?.PrimaryEmail?.Trim() ?? string.Empty;
                }
            }

            foreach (var submission in submissions)
            {
                if (!submission.QuestionnaireId.HasValue || submission.QuestionnaireId.Value == Guid.Empty)
                    continue;
                if (submission.Answers == null || !submission.Answers.Any())
                    continue;

                var responseDto = new ResponseDto
                {
                    QuestionnaireId = submission.QuestionnaireId,
                    PostPositionId = postPositionId,
                    CreatedUserID = createdUserId,
                    EditedUserID = createdUserId,
                    SubmittedOn = now
                };

                // Edit flow bypass: if a response already exists for this questionnaire + post position (+ submitter),
                // update that response instead of inserting a new duplicate.
                var existingResponses = await _responseService.GetAllAsync(
                    filter: r =>
                        r.QuestionnaireId == submission.QuestionnaireId &&
                        r.PostPositionId == postPositionId &&
                        (string.IsNullOrWhiteSpace(submitterEmail) || r.SubmitterEmail == submitterEmail),
                    includes: null,
                    pageParam: null);

                var existingResponse = existingResponses.FirstOrDefault();
                if (existingResponse?.ResponseId.HasValue == true && existingResponse.ResponseId.Value != Guid.Empty)
                {
                    responseDto.ResponseId = existingResponse.ResponseId;
                }

                var upsertedResponse = await _responseService.UpsertAsync(responseDto);
                if (upsertedResponse?.ResponseId == null)
                    continue;

                foreach (var answerItem in submission.Answers)
                {
                    if (!answerItem.QuestionId.HasValue || answerItem.QuestionId.Value == Guid.Empty)
                        continue;

                    var answerDto = new AnswersDto
                    {
                        ResponseId = upsertedResponse.ResponseId,
                        QuestionId = answerItem.QuestionId,
                        Answer = answerItem.Answer ?? string.Empty
                    };
                    await _answersService.UpsertAsync(answerDto);
                }
            }
        }

        /// <summary>
        /// Calculates vehicle/equipment charges for one closed-post-position history row using saved
        /// ClosedPostPositionEquipment records and DepartmentEquipment pricing rules.
        /// </summary>
        private async Task<(decimal vehicleCharge, decimal equipmentCharge)> CalculateEquipmentChargesForClosedPostPositionAsync(Guid closedPostPositionId)
        {
            if (closedPostPositionId == Guid.Empty)
                return (0m, 0m);

            var equipments = (await GetAllAsync(
                x => x.ClosedPostPositionID == closedPostPositionId,
                null,
                null))?.ToList() ?? new List<ClosedPostPositionEquipmentDto>();

            var closedPostPosition = await _closedPostPositionService.GetByIdAsync(closedPostPositionId).ConfigureAwait(false);
            var billableHoursForUsage = closedPostPosition?.IsMinimumDue == true
                ? closedPostPosition.GetBillableHours()
                : (decimal?)null;

            return await CalculateEquipmentChargesAsync(
                equipments,
                closedPostPosition?.PostID,
                billableHoursForUsage)
                .ConfigureAwait(false);
        }

        private async Task<(decimal vehicleCharge, decimal equipmentCharge)> CalculateEquipmentChargesAsync(
            IReadOnlyList<ClosedPostPositionEquipmentDto> equipments,
            Guid? postId = null,
            decimal? billableHoursForUsage = null)
        {
            if (equipments.Count == 0)
                return (0m, 0m);

            postId ??= equipments
                .FirstOrDefault(e => e.PostID.HasValue && e.PostID.Value != Guid.Empty)?
                .PostID;

            var isEscortActivityType = false;
            if (postId is Guid resolvedPostId && resolvedPostId != Guid.Empty)
            {
                var post = await _postService.GetByIdAsync(resolvedPostId, p => p.DepartmentActivityType!).ConfigureAwait(false);
                isEscortActivityType = post?.IsEscortActivityType == true;
            }

            var departmentEquipmentIds = equipments
                .Where(e => e.DepartmentEquipmentID.HasValue && e.DepartmentEquipmentID.Value != Guid.Empty)
                .Select(e => e.DepartmentEquipmentID!.Value)
                .Distinct()
                .ToList();

            if (departmentEquipmentIds.Count == 0)
                return (0m, 0m);

            Expression<Func<DepartmentEquipment, object>>[] includes = { x => x.EquipmentType };
            var deptEquips = await _departmentEquipmentService.GetAllAsync(
                de => departmentEquipmentIds.Contains(de.DepartmentEquipmentID),
                includes,
                null);

            var deptEquipDict = (deptEquips ?? Enumerable.Empty<DepartmentEquipmentDto>())
                .Where(de => de.DepartmentEquipmentID.HasValue)
                .GroupBy(de => de.DepartmentEquipmentID!.Value)
                .ToDictionary(g => g.Key, g => g.First());

            decimal vehicleCharge = 0m;
            decimal equipmentCharge = 0m;

            foreach (var cpe in equipments)
            {
                if (!cpe.DepartmentEquipmentID.HasValue)
                    continue;
                if (!deptEquipDict.TryGetValue(cpe.DepartmentEquipmentID.Value, out var equip) || equip == null)
                    continue;

                if (equip.EquipmentType?.IsVehicle == true)
                {
                    vehicleCharge += InvoiceDetailDto.CalculateVehicleEquipmentFee(cpe, equip, isEscortActivityType, billableHoursForUsage);
                }
                else
                {
                    var charge = equip.Charge ?? 0m;
                    if (cpe.EquipmentUsedHours.HasValue)
                    {
                        var hours = billableHoursForUsage ?? (cpe.EquipmentUsedHours.Value / 60m);
                        equipmentCharge += hours * charge;
                    }
                    else if (cpe.UsedEquipmentQuantity.HasValue)
                    {
                        equipmentCharge += cpe.UsedEquipmentQuantity.Value * charge;
                    }
                }
            }

            return (vehicleCharge, equipmentCharge);
        }

        /// <summary>
        /// When <see cref="SplitPostPositionRequestDto.IsSplitWithSelf"/> is true, bypass duplicate-assignment availability only.
        /// </summary>
        private static bool IsDuplicateJobAssignmentOnDateAvailabilityError(string? message, SplitPostPositionRequestDto request)
        {
            if (request?.IsSplitWithSelf != true)
                return true;

            var template = ModelValidationErrorCodes.GetErrorMessage(
                ModelValidationErrorCodes.DUPLICATE_JOB_POSITION_ASSIGNMENT_FOR_EMPLOYEE);

            return message.EndsWith(template, StringComparison.OrdinalIgnoreCase);
        }

        private async Task RollbackStatusAsync(
            Guid postPositionId,
            PostPositionStatus originalStatus,
            bool originalIsSplit,
            Guid? actionEmployeeId,
            string? actionUsername)
        {
            await _postPositionService.UpdateAsync(
                filter: x => x.PostPositionID == postPositionId,
                updateAction: pp => {
                    pp.PostPositionStatus = originalStatus;
                    pp.IsSplit = originalIsSplit;
                    pp.EditedDate = DateTime.UtcNow;
                    pp.EditedUserID = actionEmployeeId;
                    pp.EditedBy = actionUsername;
                }).ConfigureAwait(false);
        }

        private static void ValidateEquipmentMileageBeforeClose(SplitPostPositionRequestDto request)
        {
            if (request.EquipmentDetails == null || !request.EquipmentDetails.Any())
                return;

            foreach (var equipment in request.EquipmentDetails)
            {
                if (!equipment.DepartmentEquipmentID.HasValue)
                    continue;

                if (equipment.StartMileage.HasValue && equipment.EndMileage.HasValue && equipment.EndMileage.Value <= equipment.StartMileage.Value)
                {
                    throw new WebApiException(
                        HttpStatusCode.BadRequest,
                        ((int)HttpStatusCode.BadRequest).ToString(),
                        ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE);
                }
            }
        }

        /// <summary>
        /// Declines a split post position, records history, and creates a new available post position.
        /// </summary>
        // DeclineSplitAsync moved to PostPositionService.
    }
}
