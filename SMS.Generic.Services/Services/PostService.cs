using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SMS.Generic.Common;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Net;

namespace SMS.Generic.Services.Services
{
    /// <summary>
    /// Service for managing post operations using the generic service pattern.
    /// </summary>
    public class PostService : GenericService<PostDto, Post>, IPostService
    {
        private static readonly ConcurrentDictionary<Guid, (PostDto Before, PostDto After, string OldVehicleDetails)> PendingScheduleEmails = new();

        private readonly IPostRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITokenParser _tokenParser;
        private readonly ISkillService _skillService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IUploadFileService _uploadFileService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmployeeTypeService _employeeTypeService;
        private readonly IPostCallSignService _postCallSignService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostService"/> class.
        /// </summary>
        /// <param name="repository">The post repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="skillService">The skill service (used to validate SkillIDs).</param>
        public PostService(
            IPostRepository repository,
            IMapper mapper,
            ILogger<PostService> logger,
            ITokenParser tokenParser,
            ISkillService skillService,
            IServiceScopeFactory scopeFactory,
            IUploadFileService uploadFileService,
            IEmployeeService employeeService,
            IEmailTemplateService emailTemplateService,
            IEmployeeTypeService employeeTypeService,
            IPostCallSignService postCallSignService
            ) : base(repository, mapper, "PostID", tokenParser)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _tokenParser = tokenParser;
            _skillService = skillService;
            _scopeFactory = scopeFactory;
            _uploadFileService = uploadFileService;
            _employeeService = employeeService;
            _emailTemplateService = emailTemplateService;
            _employeeTypeService = employeeTypeService;
            _postCallSignService = postCallSignService;
        }

        public async Task SyncPremiumPaidEventFromRequestAsync(Guid requestId, bool isPremiumPaidEvent)
        {
            var ratesByEmployeeTypeId = await GetEmployeeTypeRatesForRequestAsync(requestId, isPremiumPaidEvent).ConfigureAwait(false);

            await UpdateAsync(
                p => p.RequestID == requestId,
                p =>
                {
                    p.IsPremiumPaidEvent = isPremiumPaidEvent;
                    if (p.EmployeeTypeID.HasValue
                        && ratesByEmployeeTypeId.TryGetValue(p.EmployeeTypeID.Value, out var premiumRate))
                    {
                        p.PostPayRate = premiumRate;
                    }
                }).ConfigureAwait(false);
        }

        public override async Task<PostDto> UpsertAsync(PostDto dto, bool isSuperUser = false)
        {
            ValidateDtoParameter(dto);

            if (dto.CallSigns?.Any(s => !string.IsNullOrWhiteSpace(s)) == true)
                dto.DefineCallSign = true;

            PostDto? existingPostDto = null;
            string oldVehicleDetails = string.Empty;
            bool pendingScheduleUpdateEmail = false;

            if (dto.PostID.HasValue && dto.PostID.Value != Guid.Empty)
            {
                existingPostDto = await GetByIdAsync(dto.PostID.Value).ConfigureAwait(false);
                if (existingPostDto == null)
                {
                    throw new KeyNotFoundException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_NOT_FOUND_BY_ID, dto.PostID));
                }

                if (dto.TotalPostPositions < existingPostDto.TotalPostPositions
                    && !dto.ConfirmHardDeletePostCallSign
                    && dto.DefineCallSign != false)
                {
                    await EnsureCanDecreaseTotalPostPositionsAsync(dto.PostID.Value, dto.TotalPostPositions)
                        .ConfigureAwait(false);
                }

                if (existingPostDto.PostRequiresVehicle || dto.PostRequiresVehicle)
                    oldVehicleDetails = await GetPostVehicleDetailsAsync(dto.PostID.Value).ConfigureAwait(false);

                if (HasScheduleTimingChange(existingPostDto, dto))
                {
                    var impactedEmployees = await GetScheduledAssignmentWarningsAsync(
                        dto.PostID.Value,
                        existingPostDto.PostStartTime);

                    if (impactedEmployees.Count > 0)
                    {
                        if (!dto.ConfirmScheduleChanges)
                        {
                            dto.HasAssignmentWarning = true;
                            dto.AssignmentWarningMessage = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_SCHEDULED_ASSIGNMENTS_IMPACT_WARNING);
                            dto.AssignmentWarningEmployees = impactedEmployees;
                            return dto;
                        }

                        await UpdateScheduledPostPositionsAsync(
                                existingPostDto,
                                dto,
                                updatePostPositions: true,
                                sendEmail: false,
                                oldVehicleDetails: oldVehicleDetails,
                                usePostStartTimeForTwoHourRule: true)
                            .ConfigureAwait(false);
                        pendingScheduleUpdateEmail = true;
                    }
                }
            }

            // Child post (PostParentID != null): validate parent exists
            if (dto.PostParentID.HasValue)
            {
                var parentPost = await GetByIdAsync(dto.PostParentID.Value).ConfigureAwait(false);
                if (parentPost == null)
                    throw new KeyNotFoundException(
                        ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_PARENT_NOT_FOUND, dto.PostParentID));
            }

            // Validate SkillIDs if provided
            if (dto.SkillIDs != null && dto.SkillIDs.Any())
            {
                await _skillService.ValidateSkillIdsAsync(dto.SkillIDs);
            }

            var isNewPost = !dto.PostID.HasValue || dto.PostID.Value == Guid.Empty;
            if (isNewPost && dto.RequestID.HasValue && dto.RequestID.Value != Guid.Empty)
            {
                using var scope = _scopeFactory.CreateScope();
                var request = await scope.ServiceProvider
                    .GetRequiredService<IRequestService>()
                    .GetByIdAsync(dto.RequestID.Value)
                    .ConfigureAwait(false);
                if (request?.IsPremiumPaidEvent == true)
                    dto.IsPremiumPaidEvent = true;
            }

            var saved = await base.UpsertAsync(dto);

            if (saved.PostID.HasValue && saved.PostID.Value != Guid.Empty)
            {
                if (dto.DefineCallSign == false)
                {
                    await _postCallSignService
                        .SoftDeleteByPostIdAsync(saved.PostID.Value)
                        .ConfigureAwait(false);
                }
                else
                {
                    await SyncPostCallSignsAsync(
                            saved.PostID.Value,
                            dto.TotalPostPositions,
                            dto.CallSigns,
                            dto.ConfirmHardDeletePostCallSign ? dto.HardDeletePostCallSignIDs : null)
                        .ConfigureAwait(false);
                }
            }

            if (existingPostDto != null && saved.PostID is Guid savedPostId)
            {
                await TryDeferOrSendPostScheduleEmailAsync(
                        existingPostDto,
                        saved,
                        savedPostId,
                        oldVehicleDetails,
                        pendingScheduleUpdateEmail)
                    .ConfigureAwait(false);

                if (existingPostDto.PostRequiresVehicle != saved.PostRequiresVehicle
                    || existingPostDto.PostRequiresEquipment != saved.PostRequiresEquipment)
                    await NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(savedPostId).ConfigureAwait(false);

                if (existingPostDto.IsExemptHours != saved.IsExemptHours)
                {
                    using var exemptScope = _scopeFactory.CreateScope();
                    await exemptScope.ServiceProvider.GetRequiredService<IPostPositionService>()
                        .UpdateFuturePostPositionExemptFlagForPostAsync(savedPostId, saved.IsExemptHours)
                        .ConfigureAwait(false);
                }
            }

            if (saved.PostID is Guid postId && postId != Guid.Empty && (isNewPost || dto.IsCloned))
                await NotifyJobsPostedAsync(postIds: postId).ConfigureAwait(false);

            return saved;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteVehicleEquipmentChangeAsync<T>(Guid postId, bool isVehicle, Func<Task<T>> saveAsync)
        {
            var postDto = await GetByIdAsync(postId).ConfigureAwait(false);
            var requiresVehicle = postDto?.PostRequiresVehicle == true;

            var oldVehicleDetails = requiresVehicle && isVehicle
                ? await GetPostVehicleDetailsAsync(postId).ConfigureAwait(false)
                : string.Empty;

            var result = await saveAsync().ConfigureAwait(false);

            if (postDto == null || !requiresVehicle || !isVehicle)
                return result;

            var newVehicleDetails = await GetPostVehicleDetailsAsync(postId).ConfigureAwait(false);

            if (PendingScheduleEmails.TryRemove(postId, out var pending))
            {
                await UpdateScheduledPostPositionsAsync(
                        pending.Before,
                        pending.After,
                        updatePostPositions: false,
                        validateChanges: false,
                        sendEmail: true,
                        oldVehicleDetails,
                        newVehicleDetails)
                    .ConfigureAwait(false);
                return result;
            }

            var beforeDto = _mapper.Map<PostDto>(postDto);
            if (string.IsNullOrEmpty(oldVehicleDetails) && !string.IsNullOrEmpty(newVehicleDetails))
                beforeDto.PostRequiresVehicle = false;

            await UpdateScheduledPostPositionsAsync(
                    beforeDto,
                    postDto,
                    updatePostPositions: false,
                    validateChanges: true,
                    sendEmail: true,
                    oldVehicleDetails,
                    newVehicleDetails)
                .ConfigureAwait(false);

            return result;
        }

        private Task TryDeferOrSendPostScheduleEmailAsync(
            PostDto beforeDto,
            PostDto afterDto,
            Guid postId,
            string oldVehicleDetails,
            bool pendingScheduleUpdateEmail)
        {
            var isYesToNo = beforeDto.PostRequiresVehicle && !afterDto.PostRequiresVehicle;
            var isNoToYes = !beforeDto.PostRequiresVehicle && afterDto.PostRequiresVehicle;

            if (!isYesToNo && !HasScheduleImpactingChangeForEmail(beforeDto, afterDto))
                return Task.CompletedTask;

            // Defer only when vehicle required changes No→Yes; equipment must be saved before that email.
            if (isNoToYes)
            {
                PendingScheduleEmails[postId] = (beforeDto, afterDto, oldVehicleDetails);
                return Task.CompletedTask;
            }

            return UpdateScheduledPostPositionsAsync(
                beforeDto,
                afterDto,
                updatePostPositions: false,
                validateChanges: !pendingScheduleUpdateEmail,
                sendEmail: true,
                oldVehicleDetails,
                newVehicleDetails: null);
        }

        private static bool HasScheduleTimingChange(PostDto beforeDto, PostDto afterDto) =>
            beforeDto.PostStartTime != afterDto.PostStartTime
            || beforeDto.PostEndTime != afterDto.PostEndTime
            || beforeDto.PostMinimumHoursRequired != afterDto.PostMinimumHoursRequired;

        private static bool HasScheduleImpactingChangeForEmail(PostDto beforeDto, PostDto afterDto) =>
            !string.Equals(beforeDto.PostLocation?.Trim(), afterDto.PostLocation?.Trim(), StringComparison.OrdinalIgnoreCase)
            || HasScheduleTimingChange(beforeDto, afterDto)
            || beforeDto.PostRequiresVehicle != afterDto.PostRequiresVehicle;

        private async Task<string> GetPostVehicleDetailsAsync(Guid postId)
        {
            using var scope = _scopeFactory.CreateScope();
            var items = await scope.ServiceProvider
                .GetRequiredService<IPostEquipmentService>()
                .GetByPostIdWithNamesAsync(postId)
                .ConfigureAwait(false);

            return string.Join(
                ", ",
                items
                    .Where(v => v.IsVehicle)
                    .OrderBy(v => v.EquipmentName)
                    .ThenBy(v => v.PostEquipmentID)
                    .Select(v => $"<b>{(string.IsNullOrWhiteSpace(v.EquipmentName) ? "Vehicle" : v.EquipmentName.Trim())}</b>"));
        }

        private async Task<string> BuildVehicleChangeSummaryForEmailAsync(
            PostDto beforeDto,
            PostDto afterDto,
            Guid postId,
            string? oldVehicleDetails,
            string? newVehicleDetails)
        {
            async Task<string> ResolveSideAsync(bool required, string? provided)
            {
                if (!required)
                    return string.Empty;
                if (provided != null)
                    return provided;
                return await GetPostVehicleDetailsAsync(postId).ConfigureAwait(false);
            }

            string oldDetails;
            string newDetails;
            if (!beforeDto.PostRequiresVehicle && !afterDto.PostRequiresVehicle)
            {
                oldDetails = newDetails = string.Empty;
            }
            else
            {
                oldDetails = await ResolveSideAsync(beforeDto.PostRequiresVehicle, oldVehicleDetails).ConfigureAwait(false);
                newDetails = await ResolveSideAsync(afterDto.PostRequiresVehicle, newVehicleDetails).ConfigureAwait(false);
            }

            if (beforeDto.PostRequiresVehicle == afterDto.PostRequiresVehicle
                && oldDetails == newDetails)
            {
                if (!afterDto.PostRequiresVehicle)
                    return string.Empty;

                return string.IsNullOrWhiteSpace(newDetails)
                    ? PostScheduleEmailMessages.VehicleAssignmentsUnchanged
                    : $"{PostScheduleEmailMessages.VehicleAssignmentsUnchanged} {PostScheduleEmailMessages.VehicleAssignedSuffix} {newDetails}";
            }

            if (beforeDto.PostRequiresVehicle && !afterDto.PostRequiresVehicle)
            {
                var change = string.Format(
                    PostScheduleEmailMessages.VehicleRequiredChangeFormat,
                    PostScheduleEmailMessages.BoldYes,
                    PostScheduleEmailMessages.BoldNo,
                    string.Empty).TrimEnd();

                return string.IsNullOrWhiteSpace(oldDetails)
                    ? change
                    : $"{change} {PostScheduleEmailMessages.VehicleRemovedSuffix} {oldDetails}";
            }

            if (!beforeDto.PostRequiresVehicle && afterDto.PostRequiresVehicle)
            {
                return string.Format(
                    PostScheduleEmailMessages.VehicleRequiredChangeFormat,
                    PostScheduleEmailMessages.BoldNo,
                    PostScheduleEmailMessages.BoldYes,
                    $"{PostScheduleEmailMessages.VehicleAssignedSuffix} {newDetails}");
            }

            return string.Format(
                PostScheduleEmailMessages.VehicleAssignmentsChangedFormat,
                oldDetails,
                newDetails);
        }

        public async Task NotifyJobsPostedAsync(Guid? requestId = null, params Guid[] postIds)
        {
            var isRequestLevel = requestId is Guid rid && rid != Guid.Empty && postIds.Length == 0;
            var ids = postIds.Length > 0
                ? postIds.Where(id => id != Guid.Empty).ToArray()
                : requestId is Guid rid2 && rid2 != Guid.Empty
                    ? (await GetAllAsync(p => p.RequestID == rid2, null, null).ConfigureAwait(false))
                        .Select(p => p.PostID ?? Guid.Empty)
                        .Where(id => id != Guid.Empty)
                        .ToArray()
                    : [];

            if (ids.Length == 0) return;

            using var scope = _scopeFactory.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var primaryPostId = ids.First();
            var emailType = isRequestLevel ? EmailType.RequestReleasedEmail : EmailType.JobsPostedEmail;
            await notificationService.SendEmployeePreferenceNotificationsAsync(true, primaryPostId, emailType).ConfigureAwait(false);
        }

        /// <summary>
        /// Blocks decreasing <see cref="PostDto.TotalPostPositions"/> when the new total
        /// is below the count of <c>PostCallSign</c> rows that already have a CallSign value (HTTP 409).
        /// Decreasing into empty (no-value) slots does not conflict.
        /// Includes <see cref="PostDecreaseTotalPositionsConflictDto.RequiresHardDeletePostCallSign"/>
        /// and valued call signs so the UI can multi-select which to hard-delete on retry.
        /// </summary>
        private async Task EnsureCanDecreaseTotalPostPositionsAsync(Guid postId, int newTotalPostPositions)
        {
            using var scope = _scopeFactory.CreateScope();
            var postCallSignService = scope.ServiceProvider.GetRequiredService<IPostCallSignService>();

            var callSignsWithValue = (await postCallSignService.GetAllAsync(
                    x => x.PostID == postId,
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false))
                .Where(x => x.PostCallSignID.HasValue
                    && x.PostCallSignID.Value != Guid.Empty
                    && !string.IsNullOrWhiteSpace(x.CallSign))
                .OrderBy(x => x.CreatedDate ?? DateTime.MaxValue)
                .ThenBy(x => x.PostCallSignID)
                .Select(x => new PostCallSignDto
                {
                    PostCallSignID = x.PostCallSignID,
                    PostID = x.PostID,
                    CallSign = x.CallSign
                })
                .ToList();

            // New total still covers all valued call signs — no conflict.
            if (newTotalPostPositions >= callSignsWithValue.Count)
                return;

            var message = ModelValidationErrorCodes.GetErrorMessage(
                ModelValidationErrorCodes.JOB_CANNOT_DECREASE_TOTAL_POST_POSITIONS_WITH_ACTIVITY);

            throw new WebApiException(
                HttpStatusCode.Conflict,
                ((int)HttpStatusCode.Conflict).ToString(),
                message,
                new PostDecreaseTotalPositionsConflictDto
                {
                    Message = message,
                    RequiresHardDeletePostCallSign = true,
                    CallSigns = callSignsWithValue
                });
        }

        private async Task SyncPostCallSignsAsync(
            Guid postId,
            int totalPostPositions,
            List<string?>? callSigns,
            List<Guid>? hardDeletePostCallSignIds = null)
        {
            using var scope = _scopeFactory.CreateScope();
            var postCallSignService = scope.ServiceProvider.GetRequiredService<IPostCallSignService>();

            var existing = (await postCallSignService.GetAllAsync(
                    x => x.PostID == postId,
                    includes: null,
                    pageParam: null)
                .ConfigureAwait(false))
                .Where(x => x.PostCallSignID.HasValue && x.PostCallSignID.Value != Guid.Empty)
                .OrderBy(x => x.CreatedDate ?? DateTime.MaxValue)
                .ThenBy(x => x.PostCallSignID)
                .ToList();

            var idsToRemove = new HashSet<Guid>();
            if (hardDeletePostCallSignIds != null && hardDeletePostCallSignIds.Count > 0)
            {
                var existingIds = existing.Select(x => x.PostCallSignID!.Value).ToHashSet();
                foreach (var id in hardDeletePostCallSignIds.Where(existingIds.Contains))
                    idsToRemove.Add(id);
            }

            var keepers = existing
                .Where(x => !idsToRemove.Contains(x.PostCallSignID!.Value))
                .ToList();

            for (var i = Math.Max(totalPostPositions, 0); i < keepers.Count; i++)
                idsToRemove.Add(keepers[i].PostCallSignID!.Value);

            if (idsToRemove.Count > 0)
            {
                await postCallSignService.UpdateAsync(
                        x => idsToRemove.Contains(x.PostCallSignID),
                        x => PopulateSoftDeleteFields(x))
                    .ConfigureAwait(false);
            }

            if (totalPostPositions <= 0)
                return;

            keepers = existing
                .Where(x => !idsToRemove.Contains(x.PostCallSignID!.Value))
                .ToList();
            var items = new List<PostCallSignDto>();
            for (var i = 0; i < totalPostPositions; i++)
            {
                var callSign = callSigns != null && i < callSigns.Count
                    ? callSigns[i]
                    : (i < keepers.Count ? keepers[i].CallSign : null);

                // Cleared CallSign on edit: pass existing ID with null so BulkUpsert soft-deletes the row.
                // Do not create new empty PostCallSign rows.
                if (string.IsNullOrWhiteSpace(callSign))
                {
                    if (i < keepers.Count)
                    {
                        items.Add(new PostCallSignDto
                        {
                            PostCallSignID = keepers[i].PostCallSignID,
                            PostID = postId,
                            CallSign = null
                        });
                    }

                    continue;
                }

                items.Add(i < keepers.Count
                    ? new PostCallSignDto
                    {
                        PostCallSignID = keepers[i].PostCallSignID,
                        PostID = postId,
                        CallSign = callSign
                    }
                    : new PostCallSignDto
                    {
                        PostID = postId,
                        CallSign = callSign
                    });
            }

            if (items.Count > 0)
                await postCallSignService.BulkUpsertAsync(items).ConfigureAwait(false);
        }

        private async Task<Dictionary<Guid, decimal>> GetEmployeeTypeRatesForRequestAsync(Guid requestId, bool isPremiumPaidEvent)
        {
            var employeeTypeIds = (await _repository.GetAllAsync(
                p => p.RequestID == requestId && p.EmployeeTypeID.HasValue && p.EmployeeTypeID != Guid.Empty,
                includes: null,
                pageParam: null).ConfigureAwait(false))
                .Select(p => p.EmployeeTypeID!.Value)
                .Distinct()
                .ToList();

            if (employeeTypeIds.Count == 0)
                return new Dictionary<Guid, decimal>();

            var employeeTypes = await _employeeTypeService.GetAllAsync(
                et => employeeTypeIds.Contains(et.EmployeeTypeID),
                includes: null,
                pageParam: null).ConfigureAwait(false);

            return employeeTypes.ToDictionary(
                et => et.EmployeeTypeID,
                et => isPremiumPaidEvent ? et.PremiumPayRate ?? 0m : et.PayRate ?? 0m);
        }

        /// <summary>
        /// Deletes a post and all dependent child data (including descendant posts) to satisfy FK-restrict relationships.
        /// </summary>
        public override async Task<bool> DeleteAsync<TKey>(TKey id)
        {
            if (id is not Guid postId || postId == Guid.Empty)
            {
                return await base.DeleteAsync(id).ConfigureAwait(false);
            }

            var existing = await GetByIdAsync(postId).ConfigureAwait(false);
            if (existing == null)
            {
                return false;
            }

            using var scope = _scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var postPositionService = sp.GetRequiredService<IPostPositionService>();
            var waitingPostPositionService = sp.GetRequiredService<IWaitingPostPositionService>();
            var postEquipmentService = sp.GetRequiredService<IPostEquipmentService>();
            var postCallSignService = sp.GetRequiredService<IPostCallSignService>();
            var postDocumentService = sp.GetRequiredService<IPostDocumentService>();
            var postNoteService = sp.GetRequiredService<IPostNoteService>();
            var blockDateService = sp.GetRequiredService<IBlockDateService>();
            var closedPostPositionService = sp.GetRequiredService<IClosedPostPositionService>();
            var closedPostPositionEquipmentService = sp.GetRequiredService<IClosedPostPositionEquipmentService>();
            var cancelledPostPositionService = sp.GetRequiredService<ICancelledPostPositionService>();
            var invoiceDetailService = sp.GetRequiredService<IInvoiceDetailService>();

            var postIds = await GetPostAndDescendantIdsAsync(postId).ConfigureAwait(false);

            foreach (var targetPostId in postIds)
            {
                var closedPostPositions = (await closedPostPositionService.GetAllAsync(
                    x => x.PostID == targetPostId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)).ToList();

                var closedPostPositionIds = closedPostPositions
                    .Where(x => x.ClosedPostPositionID.HasValue && x.ClosedPostPositionID.Value != Guid.Empty)
                    .Select(x => x.ClosedPostPositionID!.Value)
                    .ToHashSet();

                if (closedPostPositionIds.Any())
                {
                    await invoiceDetailService
                        .UpdateAsync(
                            x => x.ClosePostPositionID.HasValue && closedPostPositionIds.Contains(x.ClosePostPositionID.Value),
                            x => PopulateSoftDeleteFields(x))
                        .ConfigureAwait(false);
                }

                await postNoteService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await postDocumentService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await blockDateService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await postEquipmentService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await postCallSignService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await waitingPostPositionService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await closedPostPositionEquipmentService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await closedPostPositionService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await cancelledPostPositionService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

                await postPositionService.UpdateAsync(
                    x => x.PostID == targetPostId,
                    x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);
            }

            await UpdateAsync(
                x => postIds.Contains(x.PostID),
                x => PopulateSoftDeleteFields(x)).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// Returns the target post id and all descendant post ids.
        /// </summary>
        private async Task<List<Guid>> GetPostAndDescendantIdsAsync(Guid rootPostId)
        {
            var ids = new List<Guid> { rootPostId };
            var queue = new Queue<Guid>();
            queue.Enqueue(rootPostId);

            while (queue.Count > 0)
            {
                var parentId = queue.Dequeue();
                var children = (await GetAllAsync(
                    p => p.PostParentID == parentId,
                    includes: null,
                    pageParam: null).ConfigureAwait(false)).ToList();

                foreach (var child in children)
                {
                    if (!child.PostID.HasValue || child.PostID.Value == Guid.Empty)
                        continue;

                    var childId = child.PostID.Value;
                    if (ids.Contains(childId))
                        continue;

                    ids.Add(childId);
                    queue.Enqueue(childId);
                }
            }

            return ids;
        }

        public async Task<PostDto?> GetPostWithRequestAsync(Guid postId)
        {
            var postsWithRequest = await GetAllAsync(
                p => p.PostID == postId,
                new Expression<Func<Post, object>>[]
                {
                    p => p.Request!,
                    p => p.PayCode!
                });

            return postsWithRequest.FirstOrDefault();
        }

        public async Task<PostDto> ClonePostAsync(Guid postId)
        {
            if (postId == Guid.Empty)
                throw new ArgumentException("Post ID cannot be empty.", nameof(postId));

            var sourcePost = await _repository
                .GetByIdAsync(postId, "PostID", p => p.BlockDates)
                .ConfigureAwait(false);
            if (sourcePost == null)
                throw new KeyNotFoundException($"Post with ID {postId} not found.");

            var sourceTreePosts = new List<Post> { sourcePost };
            var queue = new Queue<Guid>();
            queue.Enqueue(sourcePost.PostID);

            while (queue.Count > 0)
            {
                var parentId = queue.Dequeue();
                var childPosts = (await _repository.GetAllAsync(
                    p => p.PostParentID == parentId,
                    includes: new Expression<Func<Post, object>>[] { p => p.BlockDates },
                    pageParam: null).ConfigureAwait(false))
                    .ToList();

                if (!childPosts.Any())
                    continue;

                sourceTreePosts.AddRange(childPosts);
                foreach (var child in childPosts)
                {
                    queue.Enqueue(child.PostID);
                }
            }

            var sourcePostIdsInOrder = new List<Guid>(sourceTreePosts.Count);
            var oldParentByOldPostId = new Dictionary<Guid, Guid?>(sourceTreePosts.Count);
            var sourceBlockDatesByOldPostId = new Dictionary<Guid, List<BlockDateDto>>(sourceTreePosts.Count);
            var postSeedDtos = new List<PostDto>(sourceTreePosts.Count);

            foreach (var sourceTreePost in sourceTreePosts)
            {
                sourcePostIdsInOrder.Add(sourceTreePost.PostID);
                oldParentByOldPostId[sourceTreePost.PostID] = sourceTreePost.PostParentID;
                sourceBlockDatesByOldPostId[sourceTreePost.PostID] = (sourceTreePost.BlockDates ?? new List<BlockDate>())
                    .Select(b => _mapper.Map<BlockDateDto>(b))
                    .ToList();

                var clonePostDto = _mapper.Map<PostDto>(sourceTreePost);
                clonePostDto.PrepareForClone();
                postSeedDtos.Add(clonePostDto);
            }

            var seededPosts = (await BulkUpsertAsync(postSeedDtos).ConfigureAwait(false)).ToList();

            var oldToNewPostIdMap = new Dictionary<Guid, Guid>(sourcePostIdsInOrder.Count);
            for (var i = 0; i < sourcePostIdsInOrder.Count; i++)
            {
                var newPostId = seededPosts[i].PostID;
                if (!newPostId.HasValue || newPostId.Value == Guid.Empty)
                    continue;

                oldToNewPostIdMap[sourcePostIdsInOrder[i]] = newPostId.Value;
            }

            var postsToReparent = new List<PostDto>();
            for (var i = 0; i < sourcePostIdsInOrder.Count; i++)
            {
                var oldPostId = sourcePostIdsInOrder[i];
                if (!oldParentByOldPostId.TryGetValue(oldPostId, out var oldParentId) || !oldParentId.HasValue)
                    continue;

                // If parent was also cloned, point to its new id; otherwise keep existing parent row (same request).
                var newParentId = oldToNewPostIdMap.TryGetValue(oldParentId.Value, out var mappedParentId)
                    ? mappedParentId
                    : oldParentId.Value;

                var seededPost = seededPosts[i];
                seededPost.PostParentID = newParentId;
                postsToReparent.Add(seededPost);
            }

            if (postsToReparent.Count > 0)
            {
                await BulkUpsertAsync(postsToReparent).ConfigureAwait(false);
            }

            await ClonePostEquipmentAsync(oldToNewPostIdMap).ConfigureAwait(false);

            await ClonePostCallSignsAsync(oldToNewPostIdMap).ConfigureAwait(false);

            using var blockScope = _scopeFactory.CreateScope();
            var blockDateService = blockScope.ServiceProvider.GetRequiredService<IBlockDateService>();
            var clonedBlockDates = new List<BlockDateDto>();

            foreach (var oldToNewPostId in oldToNewPostIdMap)
            {
                if (!sourceBlockDatesByOldPostId.TryGetValue(oldToNewPostId.Key, out var sourceBlockDates) || sourceBlockDates.Count == 0)
                    continue;

                foreach (var sourceBlockDate in sourceBlockDates)
                {
                    clonedBlockDates.Add(sourceBlockDate.CloneForPost(oldToNewPostId.Value));
                }
            }

            if (clonedBlockDates.Count > 0)
            {
                await blockDateService.BulkUpsertAsync(clonedBlockDates).ConfigureAwait(false);
            }

            var clonedPost = seededPosts.First();
            if (clonedPost.PostID is Guid clonedId && clonedId != Guid.Empty)
            {
                await NotifyJobsPostedAsync(postIds: clonedId).ConfigureAwait(false);
            }

            return clonedPost;
        }

        public Task ClonePostCallSignsAsync(IReadOnlyDictionary<Guid, Guid> oldToNewPostIdMap) =>
            _postCallSignService.CloneForPostsAsync(oldToNewPostIdMap);

        public async Task ClonePostEquipmentAsync(IReadOnlyDictionary<Guid, Guid> oldToNewPostIdMap)
        {
            if (oldToNewPostIdMap is null || oldToNewPostIdMap.Count == 0)
                return;

            using var scope = _scopeFactory.CreateScope();
            var postEquipmentService = scope.ServiceProvider
                .GetRequiredService<IPostEquipmentService>();

            var sourcePostIds = oldToNewPostIdMap.Keys.ToHashSet();

            var sourcePostEquipments = await postEquipmentService.GetAllAsync(
                pe => sourcePostIds.Contains(pe.PostID),
                includes: null,
                pageParam: null).ConfigureAwait(false);

            var clonedPostEquipments = sourcePostEquipments
                .Where(pe => pe.PostID.HasValue &&
                             oldToNewPostIdMap.ContainsKey(pe.PostID.Value))
                .Select(pe => pe.CloneForPost(
                    oldToNewPostIdMap[pe.PostID!.Value]))
                .ToList();

            if (clonedPostEquipments.Count == 0)
                return;

            await postEquipmentService
                .BulkUpsertAsync(clonedPostEquipments)
                .ConfigureAwait(false);
        }

        public async Task<PostPositionDto> AssignToPostMeAsync(Guid postId, Guid employeeId, DateOnly dateOfWork, bool isPlaceHolder = false, PlaceholderDto? placeholder = null, Guid? sourcePostPositionId = null, bool enableInvoiceAndPayroll = false, decimal? waitingPostPositionPayRate = null)
        {
            using var scope = _scopeFactory.CreateScope();
            var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();
            return await postPositionService.AssignToPostMeAsync(postId, employeeId, dateOfWork, isPlaceHolder, placeholder, sourcePostPositionId, enableInvoiceAndPayroll, waitingPostPositionPayRate);
        }

        private async Task<List<PostPositionDto>> GetScheduledPostPositionsAsync(
            Guid postId,
            bool includeEmployee = false,
            bool applyTwoHourEmailRule = false,
            TimeOnly postStartTime = default,
            bool usePostStartTimeForTwoHourRule = false)
        {
            var departmentNow = GetCurrentDepartmentDateTime();

            using var scope = _scopeFactory.CreateScope();
            var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();

            var includes = includeEmployee
                ? new Expression<Func<PostPosition, object>>[] { p => p.Employee, p => p.Post }
                : null;

            var scheduled = (await postPositionService.GetAllAsync(
                    p => p.PostID == postId &&
                         p.DateOfWork.HasValue &&
                         p.EmployeeID != Guid.Empty &&
                         (p.PostPositionStatus == PostPositionStatus.Assigned || p.PostPositionStatus == PostPositionStatus.CancellationPending),
                    includes: includes,
                    pageParam: null).ConfigureAwait(false))
                .Where(p => !p.IsPostPositionDateTimePassed(departmentNow))
                .ToList();

            if (!applyTwoHourEmailRule)
                return scheduled;

            return scheduled
                .Where(p => IsEligibleForScheduleAction(p, departmentNow, postStartTime, usePostStartTimeForTwoHourRule))
                .ToList();
        }

        private static bool IsWithinTwoHourScheduleWindow(DateTime startsAt, DateTime departmentNow) =>
            (startsAt - departmentNow).TotalHours < 2;

        private static bool IsEligibleForScheduleAction(
            PostPositionDto position,
            DateTime departmentNow,
            TimeOnly postStartTime,
            bool usePostStartTimeOnly)
        {
            if (position.DateOfWork is not DateOnly dateOfWork)
                return false;

            var startTime = usePostStartTimeOnly ? postStartTime : position.PostStartTime ?? postStartTime;
            return !IsWithinTwoHourScheduleWindow(dateOfWork.ToDateTime(startTime), departmentNow);
        }

        private static string? GetEmployeeRecipientEmail(EmployeeDto? employee) =>
            employee?.OffDutyCallBackEmail?.Trim() ?? employee?.PrimaryEmail?.Trim();

        private static string FormatEmployeeName(EmployeeDto? employee) =>
            PersonNameFormatHelper.FormatLastNameCommaFirstName(employee?.LastName, employee?.FirstName) ?? string.Empty;

        private async Task<List<PostAssignmentWarningEmployeeDto>> GetScheduledAssignmentWarningsAsync(Guid postId, TimeOnly postStartTime)
        {
            var scheduled = await GetScheduledPostPositionsAsync(
                    postId,
                    includeEmployee: true,
                    applyTwoHourEmailRule: true,
                    postStartTime: postStartTime,
                    usePostStartTimeForTwoHourRule: true)
                .ConfigureAwait(false);

            return scheduled
                .Select(p => new PostAssignmentWarningEmployeeDto(
                    p.EmployeeID,
                    p.PostPositionID,
                    p.DateOfWork!.Value,
                    p.PostStartTime ?? postStartTime,
                    p.PostEndTime ?? postStartTime.AddMinutes(p.LengthOfWork),
                    p.LengthOfWork,
                    FormatEmployeeName(p.Employee),
                    GetEmployeeRecipientEmail(p.Employee),
                    p.Employee?.PrimaryPhone))
                .OrderBy(x => x.DateOfWork)
                .ThenBy(x => x.StartTime)
                .ThenBy(x => x.EmployeeName)
                .ToList();
        }

        /// <summary>
        /// Updates scheduled (not-started) post positions for a post to match the new start/end time and length,
        /// then notifies assigned employees. When <paramref name="updatePostPositions"/> is false, only sends email.
        /// </summary>
        private async Task UpdateScheduledPostPositionsAsync(
            PostDto beforeDto,
            PostDto afterDto,
            bool updatePostPositions = true,
            bool validateChanges = false,
            bool sendEmail = true,
            string? oldVehicleDetails = null,
            string? newVehicleDetails = null,
            bool usePostStartTimeForTwoHourRule = false)
        {
            var postId = afterDto.PostID!.Value;
            string? vehicleChangeSummary = null;

            if (validateChanges)
            {
                vehicleChangeSummary = await BuildVehicleChangeSummaryForEmailAsync(
                    beforeDto, afterDto, postId, oldVehicleDetails, newVehicleDetails).ConfigureAwait(false);

                if (!HasScheduleImpactingChangeForEmail(beforeDto, afterDto) && string.IsNullOrEmpty(vehicleChangeSummary))
                    return;
            }

            var scheduled = await GetScheduledPostPositionsAsync(
                    postId,
                    includeEmployee: true,
                    applyTwoHourEmailRule: true,
                    postStartTime: usePostStartTimeForTwoHourRule ? beforeDto.PostStartTime : afterDto.PostStartTime,
                    usePostStartTimeForTwoHourRule: usePostStartTimeForTwoHourRule)
                .ConfigureAwait(false);

            if (scheduled.Count == 0)
                return;

            var beforeChanges = scheduled.ToDictionary(
                x => x.PostPositionID,
                x => (
                    x.DateOfWork,
                    updatePostPositions ? x.PostStartTime ?? beforeDto.PostStartTime : beforeDto.PostStartTime,
                    updatePostPositions ? x.PostEndTime ?? beforeDto.PostEndTime : beforeDto.PostEndTime,
                    updatePostPositions ? x.LengthOfWork : beforeDto.PostLength,
                    GetEmployeeRecipientEmail(x.Employee)));

            if (updatePostPositions)
            {
                foreach (var position in scheduled)
                {
                    position.PostStartTime = afterDto.PostStartTime;
                    position.PostEndTime = afterDto.PostEndTime;
                    position.LengthOfWork = afterDto.PostLength;
                }

                using var scope = _scopeFactory.CreateScope();
                var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();
                await postPositionService.BulkUpsertAsync(scheduled).ConfigureAwait(false);
            }

            if (!sendEmail)
                return;

            vehicleChangeSummary ??= await BuildVehicleChangeSummaryForEmailAsync(
                beforeDto, afterDto, postId, oldVehicleDetails, newVehicleDetails).ConfigureAwait(false);

            await SendPostScheduleUpdatedEmailsAsync(
                    scheduled,
                    beforeChanges,
                    beforeDto,
                    afterDto,
                    vehicleChangeSummary)
                .ConfigureAwait(false);
        }

        private async Task SendPostScheduleUpdatedEmailsAsync(
            IReadOnlyCollection<PostPositionDto> scheduled,
            IReadOnlyDictionary<Guid?, (DateOnly? DateOfWork, TimeOnly OldPositionStartTime, TimeOnly OldPositionEndTime, int OldDuration, string? Email)> beforeChanges,
            PostDto beforeDto,
            PostDto afterDto,
            string vehicleChangeSummary)
        {
            var recipientItems = new List<(string Email, string EmployeeName, DateOnly? DateOfWork, TimeOnly OldStart, TimeOnly OldEnd, int OldDuration)>();

            foreach (var position in scheduled)
            {
                if (!beforeChanges.TryGetValue(position.PostPositionID, out var oldValues))
                    continue;

                if (string.IsNullOrWhiteSpace(oldValues.Email))
                {
                    if (position.Employee != null)
                    {
                        await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                            position.Employee,
                            EmailType.PostScheduleUpdatedEmail,
                            CancellationToken.None).ConfigureAwait(false);
                    }
                    continue;
                }

                recipientItems.Add((
                    oldValues.Email!,
                    FormatEmployeeName(position.Employee),
                    oldValues.DateOfWork,
                    oldValues.OldPositionStartTime,
                    oldValues.OldPositionEndTime,
                    oldValues.OldDuration));
            }

            var groupedRecipients = recipientItems
                .GroupBy(x => new { x.Email, x.EmployeeName });

            foreach (var group in groupedRecipients)
            {
                var first = group.First();
                var dateText = string.Join(", ",
                    group.Select(x => x.DateOfWork)
                         .Distinct()
                         .OrderBy(x => x)
                         .Select(DateHelper.ToDisplayDate));

                var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    [EnumHelper.ToStringValue(EmailKeyword.Post)] = afterDto.PostTitle ?? string.Empty,
                    [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = first.EmployeeName,
                    [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = dateText,
                    [EnumHelper.ToStringValue(EmailKeyword.OldStartTime)] = DateHelper.ToDisplayTime24Hour(first.OldStart),
                    [EnumHelper.ToStringValue(EmailKeyword.NewStartTime)] = DateHelper.ToDisplayTime24Hour(afterDto.PostStartTime),
                    [EnumHelper.ToStringValue(EmailKeyword.OldEndTime)] = DateHelper.ToDisplayTime24Hour(first.OldEnd),
                    [EnumHelper.ToStringValue(EmailKeyword.NewEndTime)] = DateHelper.ToDisplayTime24Hour(afterDto.PostEndTime),
                    [EnumHelper.ToStringValue(EmailKeyword.OldMinimumHoursRequired)] = DateHelper.ToDisplayHoursAndMinutesFromMinutes(beforeDto.PostMinimumHoursRequired),
                    [EnumHelper.ToStringValue(EmailKeyword.NewMinimumHoursRequired)] = DateHelper.ToDisplayHoursAndMinutesFromMinutes(afterDto.PostMinimumHoursRequired),
                    [EnumHelper.ToStringValue(EmailKeyword.OldDuration)] = DateHelper.ToDisplayHoursFromMinutes(first.OldDuration),
                    [EnumHelper.ToStringValue(EmailKeyword.NewDuration)] = DateHelper.ToDisplayHoursFromMinutes(afterDto.PostLength),
                    [EnumHelper.ToStringValue(EmailKeyword.OldPostLocation)] = beforeDto.PostLocation ?? string.Empty,
                    [EnumHelper.ToStringValue(EmailKeyword.NewPostLocation)] = afterDto.PostLocation ?? string.Empty,
                    [EnumHelper.ToStringValue(EmailKeyword.VehicleChangeSummary)] = vehicleChangeSummary
                };

                var sendMailRequest = new SendMailRequestDto(
                    EmailType.PostScheduleUpdatedEmail,
                    first.Email,
                    fromEmail: null,
                    templateParameters);

                try
                {
                    await _emailTemplateService.SendMailAsync(sendMailRequest).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send grouped post schedule update email to {Email}.", first.Email);
                }
            }
        }

        /// <inheritdoc />
        public async Task NotifyFutureAssigneesOfVehicleEquipmentChangeAsync(Guid postId)
        {
            if (postId == Guid.Empty)
                return;

            try
            {
                var scheduled = await GetScheduledPostPositionsAsync(postId, includeEmployee: true).ConfigureAwait(false);
                if (scheduled.Count == 0)
                    return;

                var post = await GetByIdAsync(postId).ConfigureAwait(false);
                if (post == null)
                    return;

                var departmentNow = GetCurrentDepartmentDateTime();
                var futurePositionIds = scheduled
                    .Where(p => !p.IsPostPositionInFreezeWindow(departmentNow))
                    .Select(p => p.PostPositionID.GetValueOrDefault())
                    .Where(id => id != Guid.Empty)
                    .Distinct()
                    .ToList();

                using var scope = _scopeFactory.CreateScope();
                await scope.ServiceProvider
                    .GetRequiredService<IPostPositionVehicleService>()
                    .ReleaseAllocationsAsync(futurePositionIds)
                    .ConfigureAwait(false);

                var postEquipmentService = scope.ServiceProvider.GetRequiredService<IPostEquipmentService>();
                var lines = await postEquipmentService.GetByPostIdWithNamesAsync(postId).ConfigureAwait(false);

                var vehicles = FormatPostEquipmentDetails(lines.Where(l => l.IsVehicle));
                var equipment = FormatPostEquipmentDetails(lines.Where(l => !l.IsVehicle));
                var postTitle = post.PostTitle?.Trim() ?? string.Empty;

                var recipientItems = new List<(string Email, string EmployeeName, DateOnly? DateOfWork, TimeOnly? StartTime)>();
                foreach (var position in scheduled)
                {
                    var email = position.Employee?.OffDutyCallBackEmail?.Trim()
                        ?? position.Employee?.PrimaryEmail?.Trim();
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        if (position.Employee != null)
                        {
                            await _emailTemplateService.SendEmployeeEmailUnavailableNotificationAsync(
                                position.Employee,
                                EmailType.PostVehicleEquipmentUpdatedEmail,
                                CancellationToken.None).ConfigureAwait(false);
                        }
                        continue;
                    }

                    recipientItems.Add((
                        email!,
                        PersonNameFormatHelper.FormatLastNameCommaFirstName(
                            position.Employee?.LastName,
                            position.Employee?.FirstName) ?? string.Empty,
                        position.DateOfWork,
                        position.PostStartTime ?? post.PostStartTime));
                }

                foreach (var group in recipientItems.GroupBy(x => new { x.Email, x.EmployeeName }))
                {
                    var first = group.First();
                    var dateText = string.Join(", ",
                        group.Select(x => x.DateOfWork)
                             .Distinct()
                             .OrderBy(x => x)
                             .Select(DateHelper.ToDisplayDate));
                    var startText = DateHelper.ToDisplayTime24Hour(first.StartTime ?? post.PostStartTime);

                    var templateParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                    {
                        [EnumHelper.ToStringValue(EmailKeyword.Post)] = postTitle,
                        [EnumHelper.ToStringValue(EmailKeyword.EmployeeName)] = first.EmployeeName,
                        [EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate)] = dateText,
                        [EnumHelper.ToStringValue(EmailKeyword.PostStartTime)] = startText,
                        [EnumHelper.ToStringValue(EmailKeyword.Vehicles)] = vehicles,
                        [EnumHelper.ToStringValue(EmailKeyword.Equipment)] = equipment
                    };

                    try
                    {
                        await _emailTemplateService.SendMailAsync(
                            new SendMailRequestDto(
                                EmailType.PostVehicleEquipmentUpdatedEmail,
                                first.Email,
                                fromEmail: null,
                                templateParameters)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Failed to send post vehicle/equipment update email to {Email} for Post {PostId}.",
                            first.Email,
                            postId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed notifying future assignees of vehicle/equipment change for Post {PostId}.", postId);
            }
        }

        private static string FormatPostEquipmentDetails(IEnumerable<PostEquipmentWithNameDto> lines)
        {
            return string.Join("; ",
                lines.Select(line =>
                {
                    var name = string.IsNullOrWhiteSpace(line.EquipmentName)
                        ? "Equipment"
                        : line.EquipmentName.Trim();
                    var assignment = line.EquipmentPerPostPosition ? "Per Position" : "Per Job";
                    var start = DateHelper.ToDisplayDate(line.StartDate);
                    var end = DateHelper.ToDisplayDate(line.EndDate);
                    return $"{name} (Qty: {line.EquipmentQuantity}, {assignment}, {start} - {end})";
                }));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PostDto>> GetAllPostsByRequestIdAsync(Guid requestId)
        {
            var entities = await _repository.GetPostsByRequestIdAsync(requestId);
            return BuildPostHierarchy(_mapper.Map<List<PostDto>>(entities));
        }

        /// <inheritdoc />
        public async Task<PagedList<PostInRequestListDto>> GetPostsByRequestFilterAsync(
            RequestPostsFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var pageParam = filter.PageParam ?? new PageParam();

            var departmentId = _tokenParser.GetDepartmentID().Value;

            using var scope = _scopeFactory.CreateScope();
            var request = await scope.ServiceProvider
                .GetRequiredService<IRequestService>()
                .GetByIdAsync(filter.RequestId)
                .ConfigureAwait(false);

            if (request == null || request.DepartmentID != departmentId)
                return new PagedList<PostInRequestListDto>(pageParam);

            Expression<Func<Post, bool>> postFilter = p => p.RequestID == filter.RequestId;
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim();
                postFilter = p => p.RequestID == filter.RequestId &&
                    (p.PostTitle.Contains(term) || p.PostLocation.Contains(term));
            }

            var pagedPosts = await GetAllPagedAsync(postFilter, pageParam, null).ConfigureAwait(false);
            var orderedPosts = BuildPostHierarchy(pagedPosts.Data.ToList());
            var rows = _mapper.Map<List<PostInRequestListDto>>(orderedPosts.Where(p => p.PostID.HasValue));

            return new PagedList<PostInRequestListDto>(rows, pageParam, pagedPosts.TotalRecords);
        }

        /// <inheritdoc />
        public async Task<PagedList<PostPositionEmployeeLocationDto>> GetEmployeesWithLocationsByPostIdAsync(
            Guid postId,
            CancellationToken cancellationToken = default)
        {
            var pageParam = new PageParam();
            if (postId == Guid.Empty)
                return new PagedList<PostPositionEmployeeLocationDto>(pageParam);

            var departmentId = _tokenParser.GetDepartmentID() ?? Guid.Empty;

            var departmentNow = GetCurrentDepartmentDateTime();
            var shiftWindowStart = departmentNow.AddMinutes(-30);
            var shiftWindowEnd = departmentNow.AddMinutes(30);

            var post = await _repository.GetPostWithPositionsByPostIdAsync(
                postId,
                departmentId,
                shiftWindowStart,
                shiftWindowEnd,
                cancellationToken).ConfigureAwait(false);

            if (post == null)
                return new PagedList<PostPositionEmployeeLocationDto>(pageParam);

            bool IsEligiblePosition(PostPosition pp)
            {
                if (pp.EmployeeID is not { } empId || empId == Guid.Empty) return false;
                if (pp.PostPositionStatus is PostPositionStatus.Cancelled or PostPositionStatus.Closed or PostPositionStatus.ClosedAndSplit or PostPositionStatus.Declined) return false;
                if (pp.DateOfWork is not { } workDate) return false;

                var shiftStart = workDate.ToDateTime(pp.StartTime ?? post.PostStartTime);
                var shiftEnd = workDate.ToDateTime(pp.EndTime ?? post.PostEndTime);
                if (shiftEnd <= shiftStart) shiftEnd = shiftEnd.AddDays(1);

                return departmentNow >= shiftStart.AddMinutes(-30) && departmentNow <= shiftEnd.AddMinutes(30);
            }

            var activePositions = (post.PostPositions ?? (ICollection<PostPosition>)Array.Empty<PostPosition>())
                .Where(IsEligiblePosition)
                .ToList();

            if (activePositions.Count == 0)
                return new PagedList<PostPositionEmployeeLocationDto>(pageParam);

            var targetPostPositionIds = activePositions.Select(pp => pp.PostPositionID).ToHashSet();
            var targetEmployeeIds = activePositions
                .Where(pp => pp.EmployeeID.HasValue && pp.EmployeeID.Value != Guid.Empty)
                .Select(pp => pp.EmployeeID!.Value)
                .ToHashSet();

            IReadOnlyList<EmployeePostLocation> locations = Array.Empty<EmployeePostLocation>();
            if (targetPostPositionIds.Count > 0 || targetEmployeeIds.Count > 0)
            {
                using var scope = _scopeFactory.CreateScope();
                var requestRepo = scope.ServiceProvider.GetRequiredService<IRequestRepository>();
                locations = await requestRepo.GetLatestLocationsForPositionsAsync(
                    targetPostPositionIds,
                    targetEmployeeIds,
                    cancellationToken).ConfigureAwait(false);
            }

            var posLocDict = locations
                .Where(l => targetPostPositionIds.Contains(l.PostPositionID))
                .GroupBy(l => l.PostPositionID)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(l => new EmployeePostLocationItemDto
                    {
                        EmployeePostLocationId = l.EmployeePostLocationID,
                        EmployeeId = l.EmployeeID,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        CapturedAt = l.CapturedAt
                    }).ToList());

            var empLocDict = locations
                .GroupBy(l => l.EmployeeID)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(l => new EmployeePostLocationItemDto
                    {
                        EmployeePostLocationId = l.EmployeePostLocationID,
                        EmployeeId = l.EmployeeID,
                        Latitude = l.Latitude,
                        Longitude = l.Longitude,
                        CapturedAt = l.CapturedAt
                    }).ToList());

            var mappedEmployees = activePositions
                .Select(pp =>
                {
                    List<EmployeePostLocationItemDto>? empLocations = null;
                    if (!posLocDict.TryGetValue(pp.PostPositionID, out empLocations) && pp.EmployeeID.HasValue)
                    {
                        empLocDict.TryGetValue(pp.EmployeeID.Value, out empLocations);
                    }

                    var emp = pp.Employee;
                    return new PostPositionEmployeeLocationDto
                    {
                        PostPositionId = pp.PostPositionID,
                        DateOfWork = pp.DateOfWork,
                        StartTime = pp.StartTime ?? post.PostStartTime,
                        EndTime = pp.EndTime ?? post.PostEndTime,
                        Status = pp.PostPositionStatus,
                        EmployeeId = pp.EmployeeID,
                        EmployeeName = emp != null ? BaseDto.FormatDisplayName(emp.LastName, emp.FirstName) : string.Empty,
                        BadgeNumber = emp?.BadgeNumber,
                        EmployeeType = emp?.EmployeeType?.EmployeeTypeShort ?? emp?.EmployeeType?.EmployeeTypeLong,
                        CallSign = pp.CallSign,
                        IsPlaceholder = pp.IsPlaceholder,
                        EmployeePostLocations = empLocations != null
                            ? empLocations.Select(l => new EmployeePostLocationItemDto
                            {
                                EmployeePostLocationId = l.EmployeePostLocationId,
                                EmployeeId = l.EmployeeId != Guid.Empty ? l.EmployeeId : (pp.EmployeeID ?? Guid.Empty),
                                Latitude = l.Latitude,
                                Longitude = l.Longitude,
                                CapturedAt = l.CapturedAt
                            }).ToList()
                            : new List<EmployeePostLocationItemDto>()
                    };
                })
                .ToList();

            return new PagedList<PostPositionEmployeeLocationDto>(
                mappedEmployees,
                new PageParam { Page = 1, PageSize = mappedEmployees.Count > 0 ? mappedEmployees.Count : 25 },
                mappedEmployees.Count);
        }

        public async Task<PagedList<PostDto>> GetPostByRequestIdAsync(Guid requestId, PageParam? pagination = null)
        {
            // Fetch all posts for this request (no DB-level paging) so that
            // hierarchy can be built correctly, and then apply paging in-memory.
            var allPosts = (await GetAllAsync(
                filter: x => x.RequestID == requestId,
                includes: null,
                pageParam: null)).ToList();

            var orderedPosts = new List<PostDto>();
            BuildRecursive(allPosts, null, string.Empty, orderedPosts);

            // Populate Assignedemployeecount and related counts based on post positions.
            if (orderedPosts.Any())
            {
                using var scope = _scopeFactory.CreateScope();
                var postPositionService = scope.ServiceProvider.GetRequiredService<IPostPositionService>();

                var postIds = orderedPosts
                    .Where(p => p.PostID.HasValue)
                    .Select(p => p.PostID!.Value)
                    .ToList();

                if (postIds.Count > 0)
                {
                    var postPositions = (await postPositionService.GetAllAsync(
                        filter: pp => postIds.Contains(pp.PostID),
                        includes: null,
                        pageParam: null)).ToList();

                    var parentPostPositions = postPositions
                        .Where(pp =>
                            pp.ParentPostPositionId == null &&
                            pp.PostPositionStatus != PostPositionStatus.Cancelled &&
                            pp.PostPositionStatus != PostPositionStatus.Transferred)
                        .ToList();

                    // Existing field behavior: parent positions excluding cancelled/transferred assignments.
                    var occupiedCountsByPostId = parentPostPositions
                        .GroupBy(pp => pp.PostID)
                        .ToDictionary(g => g.Key, g => g.Count());

                    // Include rows with ParentPostPositionId set (e.g. split children); count distinct PostPositionID per post.
                    var assignedPostPositionCountsByPostId = postPositions
                        .Where(pp =>
                            (pp.PostPositionStatus == PostPositionStatus.Assigned ||
                             pp.PostPositionStatus == PostPositionStatus.CancellationPending ||
                             pp.PostPositionStatus == PostPositionStatus.Closed))
                        .GroupBy(pp => pp.PostID)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(pp => pp.PostPositionID).Distinct().Count());

                    var cancelledPostPositionCountsByPostId = postPositions
                        .Where(pp => pp.PostPositionStatus == PostPositionStatus.Cancelled)
                        .GroupBy(pp => pp.PostID)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(pp => pp.PostPositionID).Distinct().Count());

                    foreach (var post in orderedPosts)
                    {
                        if (!post.PostID.HasValue)
                            continue;

                        var postId = post.PostID.Value;
                        post.Assignedemployeecount = occupiedCountsByPostId.GetValueOrDefault(postId);
                        post.AssignedPostPositionCount = assignedPostPositionCountsByPostId.GetValueOrDefault(postId);
                        post.CancelledPostPositionCount = cancelledPostPositionCountsByPostId.GetValueOrDefault(postId);
                    }
                }
            }

            var effectivePageParam = pagination ?? new PageParam();

            // Use GenericService's GetFilteredCountAsync API for total count
            var totalRecords = await GetFilteredCountAsync(
                predicate: x => x.RequestID == requestId,
                includes: null);

            var pageSize = effectivePageParam.GetPageSize() ?? PageParam.MinPageSize;
            var page = effectivePageParam.Page ?? 1;
            var skip = (page - 1) * pageSize;

            var pagedItems = orderedPosts
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return new PagedList<PostDto>(pagedItems, effectivePageParam, totalRecords);
        }

        /// <inheritdoc />
        public List<PostDto> BuildPostHierarchy(List<PostDto> posts)
        {
            var result = new List<PostDto>();
            BuildRecursive(posts, null, string.Empty, result);
            return result;
        }

        /// <summary>
        /// Recursively calculates hierarchical levels for posts based on parent-child relationships
        /// </summary>
        /// <param name="posts">List of posts to calculate levels for</param>
        private void BuildRecursive(List<PostDto> allPosts, Guid? parentId, string parentLevel, List<PostDto> result)
        {
            var current = parentId == null
                ? allPosts.Where(x =>
                      x.PostParentID == null ||
                      !allPosts.Any(p => p.PostID == x.PostParentID))
                    .OrderBy(x => x.IsSupervisorPost)
                    .ThenBy(x => x.CreatedDate)
                    .ToList()
                : allPosts.Where(x => x.PostParentID == parentId)
                .OrderBy(x => x.IsSupervisorPost)
                .ThenBy(x => x.CreatedDate)
                .ToList();

            for (int i = 0; i < current.Count; i++)
            {
                var post = current[i];

                var level = string.IsNullOrEmpty(parentLevel)
                    ? (i + 1).ToString()
                    : $"{parentLevel}-{i + 1}";

                post.Level = level;

                result.Add(post);

                BuildRecursive(allPosts, post.PostID, level, result);
            }
        }


        /// <inheritdoc />
        public async Task<IEnumerable<PostDto>> GetSupervisorPostsAsync(Guid? requestId = null)
        {
            Expression<Func<Post, bool>> filter = requestId.HasValue
                ? (p => p.IsSupervisorPost && p.RequestID == requestId.Value)
                : (p => p.IsSupervisorPost);
            var entities = (await GetAllAsync(
                filter: filter,
                includes: null,
                pageParam: null)).ToList();
            var posts = _mapper.Map<List<PostDto>>(entities);
            var result = new List<PostDto>();
            BuildRecursive(posts, null, string.Empty, result);
            return result;
        }
        public async Task<IEnumerable<EmployeeDto>> GetEligibleEmployeesByPostIdAsync(Guid postId)
        {
            // Get post with request data
            var post = await GetPostWithRequestAsync(postId);
            if (post == null)
                throw new KeyNotFoundException(
                    ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_NOT_FOUND_BY_ID, postId));

            // Get all employees
            var allEmployees = await _employeeService.GetAllAsync(filter: null, includes: null);

            // Filter employees
            var eligibleEmployees = new List<EmployeeDto>();

            foreach (var employee in allEmployees)
            {
                if (!employee.EmployeeID.HasValue)
                    continue;
                try
                {
                    await _employeeService.ValidateEmployeeEligibilityForPostAsync(
                        postSkillIds: post.SkillIDs,
                        isRankEligible: post.RankEligible ? true : null,
                        requiredEmployeeTypeIDs: post.EmployeeTypeID,
                        specificEmployeeId: employee.EmployeeID.Value);
                }
                catch (WebApiException)
                {
                    // Employee doesn't have required skills or rank, skip them
                    continue;
                }
                catch (Exception)
                {
                    // Any other validation error, skip this employee
                    continue;
                }

                // Remove password before adding to response for security
                employee.Password = null;
                eligibleEmployees.Add(employee);
            }

            // Bind profile photo for each eligible employee (from UploadFileService)
            foreach (var emp in eligibleEmployees)
            {
                if (!emp.EmployeeID.HasValue)
                    continue;
                try
                {
                    var photoResult = await _uploadFileService.GetEmployeePhotoAsync(emp.EmployeeID.Value).ConfigureAwait(false);
                    if (photoResult.HasValue)
                    {
                        await using (photoResult.Value.Stream)
                        {
                            using var ms = new System.IO.MemoryStream();
                            await photoResult.Value.Stream.CopyToAsync(ms).ConfigureAwait(false);
                            emp.ProfilePhotoContentType = photoResult.Value.ContentType;
                            emp.ProfilePhotoBase64 = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Could not load profile photo for employee {EmployeeId}.", emp.EmployeeID);
                }
            }

            return eligibleEmployees;
        }
    }
}