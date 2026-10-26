using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.VisualBasic;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Dtos.JsonConverters;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Post operations.
    /// </summary>
    public class PostDto : BaseDto
    {
        /// <summary>
        /// Primary Key - Post ID (GUID)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("postId")]
        [System.Text.Json.Serialization.JsonPropertyName("postId")]
        public Guid? PostID { get; set; }

        /// <summary>
        /// Human-readable post code from sequence (e.g. 000001).
        /// </summary>
        public string? PostCode { get; set; }

        public Guid? RequestID { get; set; }

        public Guid? PostParentID { get; set; }

        public Guid? DepartmentActivityTypeId { get; set; }

        /// <summary>
        /// True when the post's department activity type short name is Escort.
        /// </summary>
        public bool IsEscortActivityType { get; set; }

        public bool IsSupervisorPost { get; set; }

        public int TotalPostPositions { get; set; }

        /// <summary>
        /// Optional call signs for each post slot (index aligned with TotalPostPositions). Used on create.
        /// </summary>
        public List<string?>? CallSigns { get; set; }

        /// <summary>
        /// Number of post positions currently assigned to this post.
        /// </summary>
        public int Assignedemployeecount { get; set; }

        /// <summary>
        /// Number of parent post positions with Assigned status for this post.
        /// </summary>
        public int AssignedPostPositionCount { get; set; }

        /// <summary>
        /// Number of parent post positions with Cancelled status for this post.
        /// </summary>
        public int CancelledPostPositionCount { get; set; }

        public bool RankEligible { get; set; }

        public string PostTitle { get; set; }
        public string PostDescription { get; set; }

        public TimeOnly PostStartTime { get; set; }

        public TimeOnly PostEndTime { get; set; }

        public int PostLength { get; set; }

        public bool PostIsFlexible { get; set; }

        public decimal PostPayRate { get; set; }

        public decimal PostPremiumRate { get; set; }

        public bool PostRequiresVehicle { get; set; }
        public bool PostRequiresEquipment { get; set; }

        public Guid? PayCodeID { get; set; }
        public bool IsSkillsRequired { get; set; }

        // Release mode for this post's request (copied from Request.ReleaseAs)
        public ReleaseAs ReleaseAs { get; set; } = ReleaseAs.Open;

        public List<Guid>? SkillIDs { get; set; }
        public PayCodeDto? PayCode { get; set; }
        public Guid? EmployeeTypeID { get; set; }
        /// <summary>
        /// Derived Rank from EmployeeType (read-only)
        /// </summary>
        public int? EmployeeRank { get; set; }
        /// <summary>Job rank abbreviation from the employee type selected on this post (e.g. LT, SGT, OFC).</summary>
        public string? JobRank { get; set; }
        public string? PostNotes { get; set; }
        public string PostLocation { get; set; }
        public string? Level { get; set; }
        public bool IsParent { get; set; }
        public bool IsPostRequiredDocument { get; set; }

        /// <summary>
        /// Minimum hours required for this post.
        /// </summary>
        public int PostMinimumHoursRequired { get; set; }

        /// <summary>
        /// Whether this post is exempt from hours requirements.
        /// </summary>
        public bool IsExemptHours { get; set; }

        /// <summary>
        /// When true, confirms that schedule-impacting changes are allowed and related scheduled post positions should be updated.
        /// </summary>
        public bool ConfirmScheduleChanges { get; set; }

        /// <summary>
        /// When true, confirms decreasing <see cref="TotalPostPositions"/> and hard-deleting selected PostCallSign rows
        /// after a prior 409 with <c>RequiresHardDeletePostCallSign</c>.
        /// </summary>
        public bool ConfirmHardDeletePostCallSign { get; set; }

        /// <summary>
        /// UI-only flag: when true on save after clone, sends jobs-posted email for this post.
        /// </summary>
        public bool IsCloned { get; set; }

        /// <summary>
        /// PostCallSign IDs selected in the UI to hard-delete when confirming a TotalPostPositions decrease.
        /// Count should equal the number of positions being removed.
        /// </summary>
        public List<Guid>? HardDeletePostCallSignIDs { get; set; }

        /// <summary>
        /// True when post time/minimum-hours changes impact scheduled, not-started assignments.
        /// </summary>
        public bool HasAssignmentWarning { get; set; }

        /// <summary>
        /// Warning text to show when assigned employees exist on not-started schedule rows.
        /// </summary>
        public string? AssignmentWarningMessage { get; set; }

        /// <summary>
        /// Employees assigned to impacted schedule rows (not started yet in department local time).
        /// </summary>
        public List<PostAssignmentWarningEmployeeDto> AssignmentWarningEmployees { get; set; } = new();

        /// <summary>
        /// Department response area ID (references DepartmentResponseArea table).
        /// </summary>
        public Guid? PostResponseAreaId { get; set; }

        /// <summary>
        /// Whether the post requires a uniform.
        /// </summary>
        public bool PostUniform { get; set; }

        /// <summary>
        /// Whether alcohol is served or consumed at this post.
        /// </summary>
        public bool IsAlcoholServedOrConsumed { get; set; }

        /// <summary>
        /// Whether this post is a premium paid event.
        /// </summary>
        public bool IsPremiumPaidEvent { get; set; } = false;

        /// <summary>
        /// Required rank for this post.
        /// </summary>
        public int RequiredRank { get; set; }

        /// <summary>
        /// Project number (e.g. 20419).
        /// </summary>
        public string? ProjectNumber { get; set; }

        /// <summary>
        /// Task number/name (e.g. Police).
        /// </summary>
        public string? TaskNumber { get; set; }

        /// <summary>
        /// Award number (e.g. 82187).
        /// </summary>
        public string? AwardNumber { get; set; }

        /// <summary>
        /// Fund number (e.g. 0001).
        /// </summary>
        public string? FundNumber { get; set; }

        /// <summary>
        /// Organization number (e.g. 1401385).
        /// </summary>
        public string? OrganizationNumber { get; set; }

        /// <summary>
        /// Gets or sets the first name (post contact).
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the primary phone.
        /// </summary>
        public string? PrimaryPhone
        {
            get => _primaryPhone;
            set => _primaryPhone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the cell phone.
        /// </summary>
        public string? CellPhone
        {
            get => _cellPhone;
            set => _cellPhone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the primary email.
        /// </summary>
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to define call sign.
        /// </summary>
        public bool? DefineCallSign { get; set; }

        private string? _primaryPhone;
        private string? _cellPhone;

        public string? PostDepartmentDefinedValue1 { get; set; }
        public string? PostDepartmentDefinedValue2 { get; set; }
        public string? PostDepartmentDefinedValue3 { get; set; }
        public string? PostDepartmentDefinedValue4 { get; set; }
        public string? PostDepartmentDefinedValue5 { get; set; }
        public string? PostDepartmentDefinedValue6 { get; set; }
        public string? PostDepartmentDefinedValue7 { get; set; }
        public string? PostDepartmentDefinedValue8 { get; set; }
        public string? PostDepartmentDefinedValue9 { get; set; }
        public string? PostDepartmentDefinedValue10 { get; set; }

        /// <summary>Parent request (in-memory only; excluded from API JSON to avoid circular reference).</summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public RequestDto? Request { get; set; }
        public List<BlockDateDto>? BlockDates { get; set; }
        public List<PostPositionDto> PostPositions { get; set; } = new List<PostPositionDto>();
        public List<WaitingPostPositionDto> WaitingPostPosition { get; set; } = new List<WaitingPostPositionDto>();
        public List<RosterInformationDto> CreateRosterRowsForPost(
            DateOnly dateOfWork,
            Dictionary<Guid, EmployeeDto> employees,
            IReadOnlyDictionary<Guid, string?>? vehicleNameByPositionId)
        {
            List<RosterInformationDto> rows = [];

            var positions = PostPositions?
                .Where(x => x.DateOfWork == dateOfWork)
                .ToList() ?? [];

            // No positions
            if (!positions.Any())
            {
                rows.Add(BuildRosterRowForPosition(null, null));
                return rows;
            }

            foreach (var position in positions)
            {
                employees.TryGetValue(position.EmployeeID, out var employee);
                string? vehicleName = null;
                if (position.PostPositionID is Guid positionId && positionId != Guid.Empty)
                    vehicleNameByPositionId?.TryGetValue(positionId, out vehicleName);

                rows.Add(BuildRosterRowForPosition(position, employee, vehicleName));
            }

            return rows
                .OrderBy(r => string.IsNullOrWhiteSpace(r.CallUnit) ? 1 : 0)
                .ThenBy(r => r.CallUnit)
                .ThenBy(r => r.EmployeeName)
                .ToList();
        }
        private RosterInformationDto BuildRosterRowForPosition(PostPositionDto? position, EmployeeDto? employee, string? vehicleName = null)
        {
            string? employeeName = null;
            string? employeeNumber = null;

            if (position?.IsPlaceholder == true)
            {
                employeeName =
                    position.PlaceholderDetail?.GetPlaceHolderName();

                employeeNumber =
                    position.PlaceholderDetail?.EmployeeNumber;
            }
            else if (employee != null)
            {
                employeeName = employee.GetFullName();
                employeeNumber = employee.EmployeeNumber;
            }

            return new RosterInformationDto(
                PostID,
                RequestID,
                PostParentID,
                Level,
                PostTitle,
                PostDescription,
                PostLocation,
                IsSupervisorPost,
                position?.CallSign,
                employeeName,
                employeeNumber,
                vehicleName,
                position?.PostPositionID,
                position?.PostPositionStatus,
                PayCode?.PayCodeValue 
            );
        }
        public void PrepareForClone(Guid? newRequestId = null)
        {
            PostID = Guid.Empty;
            PostCode = null;

            // Only set when cloning under a new request
            if (newRequestId.HasValue)
                RequestID = newRequestId.Value;

            PostParentID = null; // will be re-linked later
            Request = null;

            PostPositions = new List<PostPositionDto>();
            WaitingPostPosition = new List<WaitingPostPositionDto>();
            BlockDates = new List<BlockDateDto>();
        }
        public List<DateOnly> GetUnblockedDates(List<DateOnly> dates)
        {
            var validDates = new List<DateOnly>();

            if (BlockDates == null || !BlockDates.Any())
                return dates; // no blocks → all valid

            foreach (var date in dates)
            {
                // build post datetime range
                var postStart = date.ToDateTime(PostStartTime);
                var postEnd = postStart.AddMinutes(PostLength);

                bool isBlocked = BlockDates.Any(b =>
                    postStart < b.EndDateTime &&
                    b.StartDateTime < postEnd); // overlap check

                if (!isBlocked)
                    validDates.Add(date);
            }
            return validDates;
        }

        public DateTime? GetPostStartDateTime(DateOnly postDate)
        {
            if (PostStartTime == null)
                return null;

            return postDate.ToDateTime(PostStartTime);
        }
        public DateTime? GetPostEndDateTime(DateOnly postDate)
        {
            var start = GetPostStartDateTime(postDate);
            if (start == null || PostLength == null)
                return null;

            return start.Value.AddMinutes(PostLength);
        }


        /// <summary>
        /// Checks if an employee is already assigned to this post on a specific date.
        /// </summary>
        /// <param name="employeeId">The employee ID to check.</param>
        /// <param name="dateOfWork">The date of work to check.</param>
        /// <param name="externalPositions">Optional external collection of post positions to check against. If provided, uses this instead of PostPositions.</param>
        /// <param name="excludePostPositionStatus">Post position status to exclude from checking (e.g., Cancelled).</param>
        /// <returns>True if employee is already assigned to this post on the date; otherwise, false.</returns>

        public bool IsEmployeeEligibleForPost(EmployeeDto employee, out string message, DepartmentPolicyDto? departmentPolicy = null)
        {
            message = string.Empty;
            if (!RankEligible)
                return true;

            if (EmployeeRank == null || EmployeeRank == 0)
            {
                message = "This post does not have a valid rank requirement configured.";
                return false;
            }

            int employeeRank = employee.EmployeeType?.GetEmployeeRank() ?? 0;

            if (employeeRank > EmployeeRank)
            {
                message = $"{employee.GetFullName()} rank does not meet the minimum requirement for this post.";
                return false;
            }

            return true;
        }

        public AvailablePostDto BuildAvailablePost(Guid requestId, DateOnly date)
        {
            return new AvailablePostDto
            {
                RequestID = requestId,
                DateOfWork = date,
                PostID = PostID ?? Guid.Empty,
                PostTitle = PostTitle,
                RequestTitle = Request?.Title,
                RequesterID = Request?.RequesterID,
                RequesterTitle = Request?.Requester?.Name,
                PostStartTime = PostStartTime,
                PostEndTime = PostEndTime,
                PostLength = PostLength,
                PostPayRate = PostPayRate,
                PostPremiumRate = PostPremiumRate,
                PostLocation = PostLocation,
                RequestLocation = Request?.Location,
                PayCodeID = PayCodeID ?? PayCode?.PayCodeID,
                PayCodeValue = PayCode?.PayCodeValue,
                JobRank = JobRank,
                TotalPositions = TotalPostPositions,
                AvailablePositions = GetAvailableSlotsForDate(date),
                IsSpecialEvent = false,
                DefineCallSign = DefineCallSign,
            };
        }
        public int GetAvailableSlotsForDate(DateOnly date, bool excludeCancelledPostPosition = false)
        {
            if (PostPositions == null)
                return TotalPostPositions;

            int booked = PostPositions.Count(p =>
                p.IsSameWorkDate(date) && (
                p.PostPositionStatus == PostPositionStatus.Assigned ||
                p.PostPositionStatus == PostPositionStatus.SplitDecisionPending ||
                (!excludeCancelledPostPosition && p.PostPositionStatus == PostPositionStatus.CancellationPending) ||
                (p.PostPositionStatus == PostPositionStatus.Closed || p.PostPositionStatus == PostPositionStatus.Unclosed)));

            return TotalPostPositions - booked;
        }
        public bool HasAvailableSlotsForDate(Guid? postId, DateOnly date, bool excludeCancelledPostPosition = false)
        {
            return GetAvailableSlotsForDate(date, excludeCancelledPostPosition) > 0;
        }

        public (int Minutes, DateTime? Start, DateTime? End, bool HasExactSameDateTimeSlotConflict) HasTimeConflictWithAlreadyTakenPostPosition(DateOnly postDate, List<PostPositionDto>? employeeCurrentPostPosition)
        {
            if (employeeCurrentPostPosition == null || !employeeCurrentPostPosition.Any()) return (0, null, null, false);
            DateTime? postStartDateTime = GetPostStartDateTime(postDate);
            DateTime? postEndDateTime = GetPostEndDateTime(postDate);
            if (!postStartDateTime.HasValue || !postEndDateTime.HasValue)
                return (0, null, null, false);
            // New validation (added) - exact same date/time slot conflict for already taken-like assignments.
            // Existing overlap validation below remains unchanged.
            bool hasExactSameDateTimeSlotConflict = employeeCurrentPostPosition
                .Where(p => p.DateOfWork == postDate)
                .Select(p => new { Start = p.GetPostPositionStartDateTime(), End = p.GetPostPositionEndDateTime() })
                .Any(x => x.Start.HasValue && x.End.HasValue &&
                          x.Start.Value == postStartDateTime.Value &&
                          x.End.Value == postEndDateTime.Value);

            int maxOverlap = 0;
            DateTime? overlapStartResult = null;
            DateTime? overlapEndResult = null;
            foreach (var employeePostPosition in employeeCurrentPostPosition)
            {
                DateTime? existingPostPositionStartTime = employeePostPosition.GetPostPositionStartDateTime();
                DateTime? existingPostPositionEndTime = employeePostPosition.GetPostPositionEndDateTime();
                if (!existingPostPositionStartTime.HasValue || !existingPostPositionEndTime.HasValue)
                    continue;

                var overlapStart = postStartDateTime.Value > existingPostPositionStartTime.Value ? postStartDateTime.Value : existingPostPositionStartTime.Value;
                var overlapEnd = postEndDateTime.Value < existingPostPositionEndTime.Value ? postEndDateTime.Value : existingPostPositionEndTime.Value;
                if (overlapStart < overlapEnd)
                {
                    int minutes = (int)(overlapEnd - overlapStart).TotalMinutes;

                    if (minutes > maxOverlap)
                    {
                        maxOverlap = minutes;
                        overlapStartResult = overlapStart;
                        overlapEndResult = overlapEnd;
                    }
                }
            }
            return (maxOverlap, overlapStartResult, overlapEndResult, hasExactSameDateTimeSlotConflict);
        }
        public bool IsPostActive(DateOnly postDate, DateTime currentDateTime)
        {
            var start = GetPostStartDateTime(postDate);
            var end = GetPostEndDateTime(postDate);

            if (start == null || end == null)
                return false;

            // Case 1: Post has already ended → not active
            if (currentDateTime >= end)
                return false;

            // Case 2: Post has not started yet → still considered active (upcoming)
            if (currentDateTime < start)
                return true;

            // Case 3: Post is currently in progress → active
            // This also correctly handles overnight (cross-day) posts
            return currentDateTime >= start && currentDateTime < end;
        }
        private bool IsEmployeeAlreadyAssigned(Guid requestId, EmployeeDto employee, DateOnly postDate, List<PostPositionDto>? employeeCurrentPostPosition, List<WaitingPostPositionDto>? employeeCurrentWaitingPostPosition, HashSet<Guid?>? requestPostIds, bool isSpecialEvent)
        {
            var employeeId = employee.EmployeeID.Value;

            // 🔥 Waiting check
            bool inWaiting = false;
            if (employeeCurrentWaitingPostPosition != null)
            {
                if (isSpecialEvent)
                {
                    // For special events always use special event check regardless of displayWaitingList
                    inWaiting = employeeCurrentWaitingPostPosition.Any(w =>
                        w.IsSpecialEventAlreadyTaken(requestId, employeeId, postDate, requestPostIds));
                }
                else if (PostID.HasValue)
                {
                    // Normal post: safe to call .Value
                    inWaiting = employeeCurrentWaitingPostPosition.Any(w =>
                        w.IsWaitingPostPositionAlreadyTakenByEmployee(employeeId, postDate, PostID.Value));
                }
            }

            // 🔥 Post check
            bool inPosts = employeeCurrentPostPosition != null &&
                employeeCurrentPostPosition.Any(p =>
                    p.IsPostPositionAlreadyTakenByEmployee(employeeId, postDate, PostID ?? Guid.Empty) &&
                    (!isSpecialEvent || (requestPostIds != null && requestPostIds.Contains(p.PostID)))
                );

            return inWaiting || inPosts;
        }
        /// <param name="isListingOrTakeJob">When true (browsing available jobs), an exact same date/time as another taken job does not block listing. When false (pre-assign / take-job check), that case requires user confirmation per <see cref="AvailablePostResultDto.HasTimeConflictRequiringConfirmation"/>.</param>
        public AvailablePostResultDto GetAvailablePostsForDates(EmployeeDto employee, IEnumerable<DateOnly> postDates, DateTime searchDateTime, bool displayWaitingList, List<PostPositionDto>? employeeCurrentPostPosition = null, List<WaitingPostPositionDto>? employeeCurrentWaitingPostPosition = null, DepartmentPolicyDto? departmentPolicy = null, Guid? postPositionId = null, List<PostDto>? requestPosts = null, bool validatePost = true, bool isListingOrTakeJob = true, bool isPlaceHolder = false, string placeHolder = null)
        {
            var result = new AvailablePostResultDto();
            var posts = new List<AvailablePostDto>();
            IEnumerable<DateOnly> validDates;
            string employeeName = isPlaceHolder ? placeHolder : employee.GetFullName();
            if (!isPlaceHolder && validatePost && !IsEmployeeEligibleForPost(employee, out string reason, departmentPolicy))
            {
                result.Message = reason;
                return result;
            }
            if (validatePost && !isPlaceHolder)
            {
                validDates = employee.ValidateSkillsForDates(SkillIDs ?? new List<Guid>(), postDates);
                if (!validDates.Any())
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_REQUIRED_SKILLS_NOT_MET, employeeName);
                    return result;
                }
            }
            else
            {
                validDates = postDates;
            }
            bool isWaitingListActive = displayWaitingList && Request.EnableWaitingList;
            var requestPostIds = requestPosts?.Select(p => p.PostID).ToHashSet();
            bool useSpecialEventLogic = Request.IsSpecialEvent.Value && !validatePost;

            foreach (var postDate in validDates)
            {
                // ✅ SPECIAL EVENT CHECK
                if (!isPlaceHolder && useSpecialEventLogic)
                {
                    if (displayWaitingList)
                    {
                        result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_WAITING_LIST_NOT_SUPPORTED_FOR_SPECIAL_EVENT);
                        continue;
                    }

                    if (requestPosts != null && requestPosts.First().PostID != this.PostID)
                    {
                        result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_NOT_INCLUDED_IN_SELECTED_SPECIAL_EVENT_REQUEST);
                        continue;
                    }
                    if (IsEmployeeAlreadyAssigned(Request.RequestID, employee, postDate, employeeCurrentPostPosition, employeeCurrentWaitingPostPosition, requestPostIds, useSpecialEventLogic))
                    {
                        result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_JOB_POSITION_ASSIGNMENT_FOR_EMPLOYEE, employeeName);
                        continue;
                    }
                    if (isListingOrTakeJob && (requestPosts == null || !requestPosts.Any(p => p.IsPostActive(postDate, searchDateTime))))
                    {
                        result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_TIME_ENDED);
                        continue;
                    }
                    int totalPositions = requestPosts?.Sum(p => p.TotalPostPositions) ?? 0;
                    posts.Add(new AvailablePostDto
                    {
                        RequestID = Request.RequestID,
                        DateOfWork = postDate,
                        RequestTitle = Request.Title,
                        RequesterID = Request.RequesterID,
                        RequesterTitle = Request.Requester?.Name,
                        PostID = Guid.Empty,
                        PostTitle = string.Empty,
                        PostStartTime = default,
                        PostEndTime = default,
                        PostLength = 0,
                        PostPayRate = 0,
                        PostPremiumRate = 0,
                        PostLocation = string.Empty,
                        RequestLocation = Request?.Location,
                        TotalPositions = totalPositions,
                        AvailablePositions = totalPositions,
                        IsSpecialEvent = true
                    });
                    continue; // 🚨 VERY IMPORTANT (skip normal logic)
                }
                if (!isPlaceHolder && IsEmployeeAlreadyAssigned(Request.RequestID, employee, postDate, employeeCurrentPostPosition, employeeCurrentWaitingPostPosition, requestPostIds, useSpecialEventLogic))
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.DUPLICATE_JOB_POSITION_ASSIGNMENT_FOR_EMPLOYEE, employeeName);
                    continue;
                }
                bool excludeCancelledPostPosition = postPositionId.HasValue;
                bool hasSlots = HasAvailableSlotsForDate(PostID, postDate, excludeCancelledPostPosition);
                if (postPositionId.HasValue && hasSlots && PostPositions.Any(x => x.IsCanceledPostPosition(postPositionId.Value)))
                {
                    if (GetAvailableSlotsForDate(postDate, excludeCancelledPostPosition: false) > 0)
                    {
                        result.HasCancellationPendingSlotsRequiringDialog = true;
                        result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_CANCEL_PENDING_SLOTS_AVAILABLE);
                        return result;
                    }
                }
                if (!IsPostActive(postDate, searchDateTime) && isListingOrTakeJob)
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_TIME_ENDED);
                    continue;
                }

                // If browsing waiting list but this request doesn't have waiting list enabled → skip
                if (displayWaitingList && !Request.EnableWaitingList)
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_WAITING_LIST_NOT_ENABLED_FOR_REQUEST);
                    continue;
                }
                // Waiting list should list posts that are truly full (no open slot). CancellationPending is
                // surfaced on the Cancelled Jobs tab; it must not also drive "full" here or the same post
                // appears under Open → Show waiting list with 0/1 incorrectly.
                if (displayWaitingList && Request.EnableWaitingList && GetAvailableSlotsForDate(postDate, excludeCancelledPostPosition: true) > 0)
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_WAITING_LIST_AVAILABLE_ONLY_WHEN_SLOTS_FULL);
                    continue;
                }
                // waiting list ON + has slots → skip (not a waiting post yet)
                if (isWaitingListActive && hasSlots)
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_WAITING_LIST_FULL);
                    continue;
                }

                // waiting list OFF + no slots → skip
                if (!hasSlots && !isWaitingListActive)
                {
                    result.HasNoSlot = true;
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_NO_AVAILABLE_SLOTS);
                    continue;
                }
                if (!isPlaceHolder)
                {
                    var (overlapMinutes, overlapStart, overlapEnd, hasExactSameDateTimeSlotConflict) = HasTimeConflictWithAlreadyTakenPostPosition(postDate, employeeCurrentPostPosition);
                    bool isTenureBased = Request.RequestSchedules.Any(r => r.GetReleaseAsStatusByDate(postDate) == ReleaseAs.TenureBased);
                    bool bypassConflictForTenureAvailableList = isTenureBased && isListingOrTakeJob;
                    bool allowConflictConfirmation = departmentPolicy != null && departmentPolicy.OverlapJobConflictConfirmation && departmentPolicy.EnableOverlapJobConflictConfirmation;
                    if (!bypassConflictForTenureAvailableList)
                    {
                        if (hasExactSameDateTimeSlotConflict)
                        {
                            if (!allowConflictConfirmation)
                            {
                                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_OVERLAP_CONFLICT_CONFIRMATION_NOT_ENABLED_FOR_EMPLOYEE, employeeName);
                                continue;
                            }
                            else
                            {
                                string slotDate = postDate.ToString("MM/dd/yyyy");
                                string slotStartTime = overlapStart?.ToString("HH:mm") ?? PostStartTime.ToString("HH:mm");
                                string slotEndTime = overlapEnd?.ToString("HH:mm") ?? (GetPostEndDateTime(postDate)?.ToString("HH:mm") ?? "");
                                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_SAME_TIME_SLOT_ALREADY_TAKEN, slotDate, slotStartTime, slotEndTime, employeeName);
                                result.HasTimeConflictRequiringConfirmation = true;
                            }
                        }
                        // Same start/end as an already-taken post: do not throw or filter the browse list; enforce on Take Job.
                        if (!hasExactSameDateTimeSlotConflict && overlapMinutes > 0)
                        {
                            if (!allowConflictConfirmation)
                            {
                                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_OVERLAP_CONFLICT_CONFIRMATION_NOT_ENABLED_FOR_EMPLOYEE, employeeName);
                                continue;
                            }
                            else
                            {
                                int rounded = (int)(Math.Ceiling(overlapMinutes / 15.0) * 15);
                                int hours = overlapMinutes / 60;
                                int minutes = overlapMinutes % 60;
                                string startTime = overlapStart?.ToString("HH:mm") ?? "";
                                string endTime = overlapEnd?.ToString("HH:mm") ?? "";
                                // Policy allows confirmation, set flag to indicate conflict requiring confirmation
                                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.JOB_TIME_CONFLICT_WITH_TAKEN_POSITIONS, hours, minutes, startTime, endTime, employeeName);
                                result.HasTimeConflictRequiringConfirmation = true;
                            }
                        }
                    }
                }
                posts.Add(BuildAvailablePost(Request.RequestID, postDate));
            }
            result.Posts = posts;
            return result;
        }
    }

    public class PostAssignmentWarningEmployeeDto
    {
        public PostAssignmentWarningEmployeeDto(
            Guid employeeId,
            Guid? postPositionId,
            DateOnly dateOfWork,
            TimeOnly startTime,
            TimeOnly endTime,
            int duration,
            string employeeName,
            string? primaryEmail,
            string? phoneNumber)
        {
            EmployeeID = employeeId;
            PostPositionID = postPositionId;
            DateOfWork = dateOfWork;
            StartTime = startTime;
            EndTime = endTime;
            Duration = duration;
            EmployeeName = employeeName;
            PrimaryEmail = primaryEmail;
            PhoneNumber = phoneNumber;
        }

        public Guid EmployeeID { get; }
        public Guid? PostPositionID { get; }
        public DateOnly DateOfWork { get; }
        public TimeOnly StartTime { get; }
        public TimeOnly EndTime { get; }
        public int Duration { get; }
        public string EmployeeName { get; }
        public string? PrimaryEmail { get; }
        public string? PhoneNumber { get; }
    }
}
