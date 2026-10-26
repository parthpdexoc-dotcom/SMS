using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Post operations.
    /// </summary>
    public class PostPositionDto : BaseDto, IEmployeeScoped
    {
        public PostPositionDto()
        {
        }

        public PostPositionDto(
            PostPositionDto source,
            Guid employeeId,
            PostPositionStatus status,
            Guid? parentPostPositionId = null,
            string? callSing = null,
            Guid? splitPostPositionId = null,
            bool isSplit = false,
            DateOnly? dateOfWork = null,
            TimeOnly? postStartTime = null,
            TimeOnly? postEndTime = null,
            Guid? postPositionId = null)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            PostPositionID = postPositionId ?? Guid.Empty;
            PostID = source.PostID;
            EmployeeID = employeeId;
            DateOfWork = dateOfWork ?? source.DateOfWork;
            PostStartTime = postStartTime ?? source.PostStartTime;
            PostEndTime = postEndTime ?? source.PostEndTime;
            LengthOfWork = CalculateLengthOfWorkFromTimes (postStartTime,postEndTime) ?? source.LengthOfWork;
            EquipmentRequired = source.EquipmentRequired;
            CallSign = source.CallSign ?? callSing;
            PostCallSignID = source.PostCallSignID;
            PostPositionStatus = status;
            ReasonForCancel = source.ReasonForCancel;
            ParentPostPositionId = parentPostPositionId;
            SplitPostPositionID = splitPostPositionId;
            IsSplit = isSplit;
            WasUnclosed = source.WasUnclosed;
            IsAllocationFinalized = source.IsAllocationFinalized;
            IsExemptPosition = source.IsExemptPosition;
            IsPlaceholder = false;
            PlaceholderID = null;
            PlaceholderDetail = null;
            EnableInvoiceAndPayroll = source.EnableInvoiceAndPayroll;
            PostPositionPayRate = source.PostPositionPayRate;
            IsPremiumPaidEvent = source.IsPremiumPaidEvent;
            PostDepartmentDefinedFieldValue1 = source.PostDepartmentDefinedFieldValue1;
            PostDepartmentDefinedFieldValue2 = source.PostDepartmentDefinedFieldValue2;
            PostDepartmentDefinedFieldValue3 = source.PostDepartmentDefinedFieldValue3;
            PostDepartmentDefinedFieldValue4 = source.PostDepartmentDefinedFieldValue4;
            PostDepartmentDefinedFieldValue5 = source.PostDepartmentDefinedFieldValue5;
            PostDepartmentDefinedFieldValue6 = source.PostDepartmentDefinedFieldValue6;
            PostDepartmentDefinedFieldValue7 = source.PostDepartmentDefinedFieldValue7;
            PostDepartmentDefinedFieldValue8 = source.PostDepartmentDefinedFieldValue8;
            PostDepartmentDefinedFieldValue9 = source.PostDepartmentDefinedFieldValue9;
            PostDepartmentDefinedFieldValue10 = source.PostDepartmentDefinedFieldValue10;
        }

        [JsonProperty("postPositionId")]
        [JsonPropertyName("postPositionId")]
        public Guid? PostPositionID { get; set; }

        /// <summary>
        /// Human-readable post position code from sequence (e.g. 000001).
        /// </summary>
        public string? PostPositionCode { get; set; }

        public Guid PostID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid EmployeeID { get; set; } = Guid.Empty;

        /// <summary>
        /// When true, this row represents a placeholder/virtual employee assignment (no real employee).
        /// </summary>
        public bool IsPlaceholder { get; set; }

        /// <summary>
        /// Foreign key to the placeholder profile when <see cref="IsPlaceholder"/> is true.
        /// </summary>
        [JsonProperty("placeholderId")]
        [JsonPropertyName("placeholderId")]
        public Guid? PlaceholderID { get; set; }

        /// <summary>
        /// Placeholder profile for create/update; populated on read via join.
        /// </summary>
        [JsonProperty("placeholderDetail")]
        [JsonPropertyName("placeholderDetail")]
        public PlaceholderDto? PlaceholderDetail { get; set; }

        /// <summary>
        /// When true on a placeholder assignment, the post position is included in invoice (and payroll) processing.
        /// </summary>
        public bool EnableInvoiceAndPayroll { get; set; }

        /// <summary>
        /// Non-placeholder assignments are always invoice-eligible; placeholders require <see cref="EnableInvoiceAndPayroll"/>.
        /// </summary>
        public bool IsEligibleForInvoicing() => !IsPlaceholder || EnableInvoiceAndPayroll;

        public DateOnly? DateOfWork { get; set; }
        public TimeOnly? PostStartTime { get; set; }
        public TimeOnly? PostEndTime { get; set; }
        public int LengthOfWork { get; set; }
        public bool EquipmentRequired { get; set; }
        public string? CallSign { get; set; }
        public Guid? PostCallSignID { get; set; }
        public decimal PostPositionPayRate { get; set; }
        public bool IsPremiumPaidEvent { get; set; }
        public PostPositionStatus PostPositionStatus { get; set; }
        public string? ReasonForCancel { get; set; }
        public Guid? ParentPostPositionId { get; set; }
        public bool IsSplit { get; set; } = false;
        public bool WasUnclosed { get; set; }
        public Guid? SplitPostPositionID { get; set; }

        /// <summary>
        /// When true, vehicle/equipment allocation has already been snapshotted for this position
        /// (including an empty kit). The finalize cron must not run again for this row.
        /// </summary>
        public bool IsAllocationFinalized { get; set; }

        /// <summary>
        /// When true, this assignment is left out of tenure lookback totals.
        /// </summary>
        public bool IsExemptPosition { get; set; }
        public string? PostDepartmentDefinedFieldValue1 { get; set; }
        public string? PostDepartmentDefinedFieldValue2 { get; set; }
        public string? PostDepartmentDefinedFieldValue3 { get; set; }
        public string? PostDepartmentDefinedFieldValue4 { get; set; }
        public string? PostDepartmentDefinedFieldValue5 { get; set; }
        public string? PostDepartmentDefinedFieldValue6 { get; set; }
        public string? PostDepartmentDefinedFieldValue7 { get; set; }
        public string? PostDepartmentDefinedFieldValue8 { get; set; }
        public string? PostDepartmentDefinedFieldValue9 { get; set; }
        public string? PostDepartmentDefinedFieldValue10 { get; set; }
        public EmployeeDto Employee { get; set; } = null!;

        /// <summary>Parent post (in-memory only; excluded from API JSON to avoid circular reference).</summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public PostDto Post { get; set; }
        public string? JobRank { get; set; }

        /// <summary>
        /// Hours worked.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int? HoursWorked { get; set; }
        public DateTime? GetPostPositionStartDateTime()
        {
            if (DateOfWork == null || PostStartTime == null)
                return null;

            return DateOfWork.Value.ToDateTime(PostStartTime.Value);
        }
        public static int? CalculateLengthOfWorkFromTimes(TimeOnly? startTime, TimeOnly? endTime)
        {
            if (!startTime.HasValue || !endTime.HasValue)
                return null;

            var referenceDate = new DateOnly(2000, 1, 1);
            var start = referenceDate.ToDateTime(startTime.Value);
            var end = referenceDate.ToDateTime(endTime.Value);
            if (endTime.Value <= startTime.Value)
                end = end.AddDays(1);

            var minutes = (int)(end - start).TotalMinutes;
            return minutes > 0 ? minutes : null;
        }
        public DateTime? GetPostPositionEndDateTime()
        {

            var start = GetPostPositionStartDateTime();
            if (start == null) return null;
            if (LengthOfWork != null)
            {
                return start.Value.AddMinutes(LengthOfWork);
            }
            var end = DateOfWork.Value.ToDateTime(PostEndTime.Value);
            if (PostStartTime.HasValue && PostEndTime.Value <= PostStartTime.Value)
                end = end.AddDays(1);

            return end;
        }

        /// <summary>
        /// Classifies this post position relative to department-local <paramref name="departmentNow"/>.
        /// Returns null when start/end cannot be resolved or when the row is outside the tracked windows.
        /// </summary>
        public OfficerScheduleStatus? GetOfficerScheduleStatus(
            DateTime departmentNow,
            int recentlyEndedMinutes = 50,
            int upcomingHours = 8)
        {
            var start = GetPostPositionStartDateTime();
            var end = GetPostPositionEndDateTime();
            if (!start.HasValue || !end.HasValue)
                return null;

            if (start.Value <= departmentNow && departmentNow < end.Value)
                return OfficerScheduleStatus.CurrentlyWorking;

            if (end.Value > departmentNow.AddMinutes(-recentlyEndedMinutes)
                && end.Value <= departmentNow
                && start.Value <= departmentNow)
                return OfficerScheduleStatus.RecentlyEnded;

            if (departmentNow < start.Value && start.Value <= departmentNow.AddHours(upcomingHours))
                return OfficerScheduleStatus.Upcoming;

            return null;
        }
       
        public bool IsEmployeeIdMatch(Guid? employeeId)
        {
            return employeeId.HasValue && EmployeeID == employeeId.Value;
        }
        public bool IsSameWorkDate(DateOnly dateOfWork)
        {
            return DateOfWork == dateOfWork;
        }
        public bool IsPostPositionAlreadyTakenByEmployee (Guid employeeId, DateOnly dateOfWork, Guid postId)
        {
            if (IsPlaceholder)
                return false;
            return IsEmployeeIdMatch(employeeId) && IsSameWorkDate(dateOfWork) && IsPostIdMatch(postId);
        }
        public bool IsPostIdMatch(Guid postId)
        {
            return PostID == postId;
        }
        public bool IsCanceledPostPosition(Guid postpositionId)
        {
            return PostPositionStatus == PostPositionStatus.CancellationPending
                       && PostPositionID == postpositionId;
        }

        /// <summary>
        /// Checks whether this row matches ongoing/scheduled view relative to the provided department-local timestamp.
        /// Ongoing means post start datetime has already passed; scheduled means it has not passed yet.
        /// </summary>
        public bool MatchesOngoingView(bool isOngoing, DateTime departmentNow)
        {
            if (!DateOfWork.HasValue)
                return false;

            var hasPassed = IsPostPositionDateTimePassed(departmentNow);
            return isOngoing ? hasPassed : !hasPassed;
        }

        /// <summary>
        /// True when work date/time is at or before <paramref name="departmentNow"/>.
        /// Uses <see cref="PostStartTime"/> or post default start time when available.
        /// Without a start time, compares only by date.
        /// </summary>
        public bool IsPostPositionDateTimePassed(DateTime departmentNow)
        {
            if (!DateOfWork.HasValue)
                return false;

            var startTime = PostStartTime ?? Post?.PostStartTime;
            if (!startTime.HasValue)
                return DateOfWork.Value < DateOnly.FromDateTime(departmentNow);

            return departmentNow >= DateOfWork.Value.ToDateTime(startTime.Value);
        }

        /// <summary>
        /// True when this position's start time is within <paramref name="hoursBeforeStart"/> (default 2 hours)
        /// of <paramref name="departmentNow"/>, or has already passed.
        /// </summary>
        public bool IsPostPositionInFreezeWindow(DateTime departmentNow, int hoursBeforeStart = 2)
        {
            if (!DateOfWork.HasValue)
                return false;

            var startTime = PostStartTime ?? Post?.PostStartTime;
            if (!startTime.HasValue)
                return DateOnly.FromDateTime(departmentNow) >= DateOfWork.Value;

            var startDateTime = DateOfWork.Value.ToDateTime(startTime.Value);
            return departmentNow.AddHours(hoursBeforeStart) >= startDateTime;
        }

        /// <summary>
        /// Same post, work date, and exact start/end window as <paramref name="other"/> (aligned with available-post exact slot match).
        /// </summary>
        public bool IsSamePostSlotAs(PostPositionDto? other)
        {
            if (other == null)
                return false;
            if (PostID != other.PostID || DateOfWork != other.DateOfWork)
                return false;

            var start = GetPostPositionStartDateTime();
            var end = GetPostPositionEndDateTime();
            var oStart = other.GetPostPositionStartDateTime();
            var oEnd = other.GetPostPositionEndDateTime();
            return start.HasValue && end.HasValue && oStart.HasValue && oEnd.HasValue
                && start.Value == oStart.Value && end.Value == oEnd.Value;
        }

        /// <summary>
        /// True if any row in <paramref name="others"/> occupies the same post/date/time window as this row.
        /// </summary>
        public bool IsSamePostSlotAsAny(IReadOnlyList<PostPositionDto>? others)
        {
            if (others == null || others.Count == 0)
                return false;
            foreach (var o in others)
            {
                if (IsSamePostSlotAs(o))
                    return true;
            }
            return false;
        }

    }
    public class BulkAssignToPostDto : PostPositionDto
    {
        public List<Guid> EmployeeIDs { get; set; } = new List<Guid>();
    }

    public class PostPositionResponseDto : PostPositionDto
    {
        public string? EmployeeName { get; set; }
        public string? BadgeNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? EmployeeTypeName { get; set; }
        public string? DepartmentName { get; set; }
        public string? EmployeeStatusName { get; set; }

        /// <summary>From the linked placeholder when this row is a placeholder assignment.</summary>
        public bool IsUniquePerPosition { get; set; }
    }

    /// <summary>
    /// Columns selected for the post-position employee list query.
    /// Mapped to <see cref="PostPositionResponseDto"/> after the query runs.
    /// </summary>
    public class PostPositionEmployeeColumnDto
    {
        public Guid PostPositionId { get; set; }
        public Guid PostId { get; set; }
        public bool IsPlaceholder { get; set; }
        public Guid? PlaceholderId { get; set; }
        public bool EnableInvoiceAndPayroll { get; set; }
        public DateOnly? DateOfWork { get; set; }
        public TimeOnly? PostStartTime { get; set; }
        public string? CallSign { get; set; }
        public Guid? PostCallSignId { get; set; }
        public PostPositionStatus PostPositionStatus { get; set; }
        public bool HasEmployee { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BadgeNumber { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? EmployeeTypeName { get; set; }
        public bool IsUniquePerPosition { get; set; }
    }

    /// <summary>
    /// Slim result item for bulk-assign-post-positions API (no nested Post/Request/Posts).
    /// </summary>
    public class BulkAssignPostPositionItemDto
    {
        public BulkAssignPostPositionItemDto()
        {
        }

        public BulkAssignPostPositionItemDto(
            Guid postId,
            DateOnly? dateOfWork,
            PostPositionStatus postPositionStatus,
            string? message,
            Guid? postPositionId = null,
            TimeOnly? postStartTime = null,
            int lengthOfWork = 0,
            bool hasTimeConflictRequiringConfirmation = false,
            Guid? employeeId = null,
            string? employeeName = null)
        {
            PostPositionId = postPositionId;
            PostId = postId;
            DateOfWork = dateOfWork;
            PostStartTime = postStartTime;
            LengthOfWork = lengthOfWork;
            PostPositionStatus = postPositionStatus;
            Message = message;
            HasTimeConflictRequiringConfirmation = hasTimeConflictRequiringConfirmation;
            EmployeeId = employeeId;
            EmployeeName = employeeName;
        }

        [JsonProperty("postPositionId")]
        [JsonPropertyName("postPositionId")]
        public Guid? PostPositionId { get; set; }
        [JsonProperty("postID")]
        [JsonPropertyName("postID")]
        public Guid PostId { get; set; }
        [JsonProperty("dateOfWork")]
        [JsonPropertyName("dateOfWork")]
        public DateOnly? DateOfWork { get; set; }
        [JsonProperty("postStartTime")]
        [JsonPropertyName("postStartTime")]
        public TimeOnly? PostStartTime { get; set; }
        [JsonProperty("lengthOfWork")]
        [JsonPropertyName("lengthOfWork")]
        public int LengthOfWork { get; set; }
        [JsonProperty("postPositionStatus")]
        [JsonPropertyName("postPositionStatus")]
        public PostPositionStatus PostPositionStatus { get; set; }
        [JsonProperty("message")]
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonProperty("hasError")]
        [JsonPropertyName("hasError")]
        public bool HasError => !string.IsNullOrEmpty(Message) && !HasTimeConflictRequiringConfirmation;

        /// <summary>
        /// When true, client should show time-overlap confirmation and retry with <c>confirmConflict: true</c>.
        /// </summary>
        [JsonProperty("hasTimeConflictRequiringConfirmation")]
        [JsonPropertyName("hasTimeConflictRequiringConfirmation")]
        public bool HasTimeConflictRequiringConfirmation { get; set; }

        [JsonProperty("employeeId")]
        [JsonPropertyName("employeeId")]
        public Guid? EmployeeId { get; set; }

        [JsonProperty("employeeName")]
        [JsonPropertyName("employeeName")]
        public string? EmployeeName { get; set; }
    }

    /// <summary>
    /// Placeholder row for bulk assign; each item carries its own invoice/payroll flag from the UI.
    /// </summary>
    public class BulkAssignPlaceholderItemDto
    {
        public Guid? PlaceholderID { get; set; }

        [JsonProperty("firstName")]
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("contactNumber")]
        [JsonPropertyName("contactNumber")]
        public string? ContactNumber { get; set; }

        [JsonProperty("employeeNumber")]
        [JsonPropertyName("employeeNumber")]
        public string? EmployeeNumber { get; set; }

        [JsonProperty("enableInvoiceAndPayroll")]
        [JsonPropertyName("enableInvoiceAndPayroll")]
        public bool EnableInvoiceAndPayroll { get; set; }

        /// <summary>
        /// When true, assigning this placeholder to another job creates a new placeholder with the same name.
        /// Badge number is not allowed.
        /// </summary>
        public bool IsUniquePerPosition { get; set; }
    }

    public class BulkAssignPostPositionRequestDto
    {
        public Guid PostID { get; set; }
        public DateOnly DateOfWork { get; set; }
        public List<Guid> EmployeeIDs { get; set; } = new List<Guid>();
        public bool IsPlaceholder { get; set; }

        /// <summary>
        /// One row per placeholder with per-row <see cref="BulkAssignPlaceholderItemDto.EnableInvoiceAndPayroll"/>.
        /// </summary>
        [JsonProperty("placeholders")]
        [JsonPropertyName("placeholders")]
        public List<BulkAssignPlaceholderItemDto> Placeholders { get; set; } = new List<BulkAssignPlaceholderItemDto>();

        /// <summary>When true, <see cref="IRequestService.AssignPostToMe"/> is invoked with the same flag to allow taking a job that overlaps the employee’s existing work (per department policy / same slot).</summary>
        [JsonProperty("confirmConflict")]
        [JsonPropertyName("confirmConflict")]
        public bool ConfirmConflict { get; set; }

        /// <summary>
        /// When true, the source assignment (<see cref="SourcePostPositionId"/>) is ended and new assignment(s) are created for <see cref="EmployeeIDs"/>.
        /// Default false preserves legacy bulk-assign behavior.
        /// </summary>
        [JsonProperty("isTransfer")]
        [JsonPropertyName("isTransfer")]
        public bool IsTransfer { get; set; }

        /// <summary>
        /// Required when <see cref="IsTransfer"/> is true: the post position row being transferred from.
        /// </summary>
        [JsonProperty("sourcePostPositionId")]
        [JsonPropertyName("sourcePostPositionId")]
        public Guid? SourcePostPositionId { get; set; }
    }
    public class BulkAssignResult
    {
        public Guid EmployeeId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = "";
    }
    public class BulkAssignPostPositionResponseDto
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
    }
    public class PostPositionOverdueDto : PostPositionDto
    {
        public decimal OverdueHours { get; set; }
    }

    /// <summary>
    /// DTO containing post title and post position data for closed/closed-and-split post positions.
    /// </summary>
    public class PostPositionWithPostTitleDto
    {
        public string PostTitle { get; set; } = string.Empty;
        public string? EmployeeName { get; set; }
        public PostPositionDto PostPosition { get; set; } = null!;
    }
    public class EmployeePolicyUsageDto
    {
        // Identity
        public Guid EmployeeId { get; set; }

        // Context (department timezone)
        public DateOnly Date { get; set; }
        public DateTime DateTime { get; set; }

        public int AppliedPostsToday { get; set; } = 0;
        // ===== Open posts =====
        public Dictionary<DateOnly, int> OpenPostsPerDay { get; set; } = new();
        public Dictionary<DateOnly, int> OpenPostsPerWeek { get; set; } = new();

        // ===== Tenure posts =====
        public Dictionary<DateOnly, int> TenurePostsPerDay { get; set; } = new();
        public Dictionary<DateOnly, int> TenurePostsPerWeek { get; set; } = new();

        // ===== Managed posts =====
        public Dictionary<DateOnly, int> ManagedPostsPerDay { get; set; } = new();
        public Dictionary<DateOnly, int> ManagedPostsPerWeek { get; set; } = new();
        /* =========================
           DATE / WEEK MAPS
           ========================= */

        // Key = work date
        public Dictionary<DateOnly, int> PostsPerDay { get; set; } = new();
        public Dictionary<DateOnly, decimal> HoursPerDay { get; set; } = new();

        // Key = week start date (department-based)
        public Dictionary<DateOnly, int> PostsPerWeek { get; set; } = new();
        public Dictionary<DateOnly, decimal> HoursPerWeek { get; set; } = new();
        public List<PostPositionOverdueDto> OverduePostPositions { get; set; } = new();


        public bool CheckDepartmentPolicy(DepartmentPolicyDto policy, DateOnly workDate, bool isHoursPolicyExempt, out string? rejectionReason)
        {
            rejectionReason = null;
            var (weekStart, weekEnd) = workDate.GetDepartmentWeekRange(policy.OnDutyStartDayOfWeek.ToDayOfWeek());
            var monthStart = new DateOnly(workDate.Year, workDate.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            /* =====================================================
               1️⃣ MONTHLY HOURS LIMIT
               ===================================================== */
            if (policy.IsHoursPerMonthWorked && policy.HoursPerMonthWorked > 0 && !isHoursPolicyExempt)
            {
                var monthHours = HoursPerDay.SumForDateRange(monthStart, monthEnd);

                if (monthHours >= policy.HoursPerMonthWorked)
                {
                    rejectionReason = "Monthly hours limit exceeded";
                    return false;
                }
            }

            /* =====================================================
               2️⃣ WEEKLY HOURS LIMIT (Department Week)
               ===================================================== */
            if (policy.IsHoursPerWeekWorked && policy.HoursPerWeekWorked > 0 && !isHoursPolicyExempt)
            {
                var weekHours = HoursPerDay.SumForDateRange(weekStart, weekEnd);

                if (weekHours >= policy.HoursPerWeekWorked)
                {
                    rejectionReason = "Weekly hours limit exceeded";
                    return false;
                }
            }
            if (policy.IsDeptOpenPostsTakenPerWeek && policy.DeptOpenPostsTakenPerWeek > 0)
            {
                var openThisWeek = OpenPostsPerWeek.GetValueOrDefault(weekStart);

                if (openThisWeek >= policy.DeptOpenPostsTakenPerWeek)
                {
                    rejectionReason = "Weekly open post limit exceeded";
                    return false;
                }
            }
            if (policy.IsTenurePostsAllowedPerWeek && policy.TenurePostsAllowedPerWeek > 0)
            {
                var tenureThisWeek = TenurePostsPerWeek.GetValueOrDefault(weekStart);
                if (tenureThisWeek >= policy.TenurePostsAllowedPerWeek)
                {
                    rejectionReason = "Weekly tenure post limit exceeded";
                    return false;
                }
            }
            /* =====================================================
               4️⃣ DAILY POST LIMIT
               ===================================================== */
            if (policy.IsPostsTakenPerDay && policy.PostsTakenPerDay > 0)
            {
                var postsOnDate = PostsPerDay.GetValueOrDefault(workDate);
                if (postsOnDate >= policy.PostsTakenPerDay)
                {
                    rejectionReason = "Daily post limit exceeded";
                    return false;
                }
            }
            if (policy.IsDeptOpenPostsTakenPerDay && policy.DeptOpenPostsTakenPerDay > 0)
            {
                var openToday = OpenPostsPerDay.GetValueOrDefault(workDate);

                if (openToday >= policy.DeptOpenPostsTakenPerDay)
                {
                    rejectionReason = "Daily open post limit exceeded";
                    return false;
                }
            }
            if (policy.IsTenurePostsAllowedPerDay && policy.TenurePostsAllowedPerDay > 0)
            {
                var tenureToday = TenurePostsPerDay.GetValueOrDefault(workDate);

                if (tenureToday >= policy.TenurePostsAllowedPerDay)
                {
                    rejectionReason = "Daily tenure post limit exceeded";
                    return false;
                }
            }
            if (policy.IsManagedPostTakenOnSameday == true &&policy.ManagedPostTakenOnSameday.HasValue)
            {
                var managedToday =
                    ManagedPostsPerDay.GetValueOrDefault(workDate);

                if (managedToday >= policy.ManagedPostTakenOnSameday.Value)
                {
                    rejectionReason = "Daily managed post limit exceeded";
                    return false;
                }
            }

            /* =====================================================
               5️⃣ DAILY HOURS LIMIT
               ===================================================== */
            if (policy.IsHoursPerDayWorked && policy.HoursPerDayWorked > 0 && !isHoursPolicyExempt)
            {
                var hoursOnDate = HoursPerDay.GetValueOrDefault(workDate);
                if (hoursOnDate >= policy.HoursPerDayWorked)
                {
                    rejectionReason = "Daily hours limit exceeded";
                    return false;
                }
            }
            return true;
        }
        public bool HasOutstandingReadyToClosePost(DepartmentPolicyDto policy)
        {
            if (policy == null || policy.TimeToClosePost <= 0)
                return false;

            return OverduePostPositions.Any(p =>
                p.OverdueHours >= policy.TimeToClosePost);
        }
        public bool HasAlreadyAppliedMaxPostToday(DepartmentPolicyDto policy)
        {
            if (policy == null || !policy.IsPostsTakenPerDay || policy.PostsTakenPerDay <= 0)
                return false;

            return AppliedPostsToday >= policy.PostsTakenPerDay;
        }
    }
}
