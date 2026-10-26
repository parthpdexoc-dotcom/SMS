using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Dtos.JsonConverters;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using static SMS.Generic.Dtos.Dtos.PostDto;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Request operations.
    /// </summary>
    public class RequestDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request.
        /// </summary>
        public Guid RequestID { get; set; }

        /// <summary>
        /// Human-readable request code from sequence (e.g. 000001).
        /// </summary>
        public string? RequestCode { get;}

        /// <summary>
        /// Gets or sets the requester ID (references Requesters table).
        /// </summary>
        public Guid? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the requester manager ID (references RequesterManager table).
        /// Null means request was created without a specific requester manager assignment.
        /// </summary>
        public Guid? RequesterManagerID { get; set; }

        /// <summary>
        /// Gets or sets the department ID (references Department table).
        /// </summary>
        public Guid DepartmentID { get; set; }


        /// <summary>
        /// Gets or sets the department response area ID (references DepartmentResponseArea table).
        /// </summary>
        public Guid? DepartmentResponseAreaID { get; set; }

        /// <summary>
        /// Gets or sets whether the waiting list is enabled for this request (replaces legacy IsTenureBasedRequest).
        /// </summary>
        public bool EnableWaitingList { get; set; } = true;

        /// <summary>
        /// Gets or sets the first name.
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
        /// Gets or sets the secondary phone.
        /// </summary>
        public string? SecondaryPhone
        {
            get => _secondaryPhone;
            set => _secondaryPhone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the alternate cell phone.
        /// </summary>
        public string? AlternateCellPhone
        {
            get => _alternateCellPhone;
            set => _alternateCellPhone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the primary email.
        /// </summary>
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets the secondary email.
        /// </summary>
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// Gets or sets whether post contact is required.
        /// </summary>
        public bool? IsPostContact { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Gets or sets the release end date.
        /// </summary>
        public DateOnly? ReleaseEndDate { get; set; }

        /// <summary>
        /// Gets or sets how the request is released.
        /// Stored as int because it maps to the ReleaseAs enum.
        /// </summary>
        public ReleaseAs ReleaseAs { get; set; } = ReleaseAs.Open;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the request site street address (required).
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request site city (required).
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request site state (required).
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request site ZIP or postal code (required).
        /// </summary>
        public string Zip { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether post locations are stationary.
        /// </summary>
        public bool? PostLocationsAreStationary { get; set; }

        /// <summary>
        /// Gets or sets whether a permit is required.
        /// </summary>
        public bool? RequiresPermit { get; set; }

        /// <summary>
        /// Gets or sets whether the request is a special event.
        /// </summary>
        public bool? IsSpecialEvent { get; set; }

        /// <summary>
        /// Gets or sets whether the special event is approved.
        /// </summary>
        public bool? SpecialEventApproved { get; set; }

        /// <summary>
        /// Gets or sets whether a liquor license is required.
        /// </summary>
        public bool? RequiresLiquorLicense { get; set; }

        /// <summary>
        /// Gets or sets whether an alcohol permit is required.
        /// </summary>
        public bool AlcoholPermitRequired { get; set; }

        /// <summary>
        /// Gets or sets the special rate for this request.
        /// </summary>
        public decimal? SpecialRate { get; set; }

        /// <summary>
        /// Gets or sets whether a vehicle is required.
        /// </summary>
        public bool? RequiresVehicle { get; set; }

        /// <summary>
        /// Gets or sets whether the vehicle requirement is approved.
        /// </summary>
        public bool? VehicleApproved { get; set; }

        /// <summary>
        /// Gets or sets whether equipment is required.
        /// </summary>
        public bool? RequiresEquipment { get; set; }
        public string? LiquorLicenseNumber { get; set; }
        public string? PermitNumber { get; set; }

        /// <summary>
        /// Gets or sets whether a skill is required.
        /// </summary>
        public bool? IsSkillRequired { get; set; }

        /// <summary>
        /// Gets or sets whether a skillIDs is required.
        /// </summary>
        public List<Guid>? SkillIDs { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 1.
        /// </summary>
        public string? DepartmentDefinedFieldValue1 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 2.
        /// </summary>
        public string? DepartmentDefinedFieldValue2 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 3.
        /// </summary>
        public string? DepartmentDefinedFieldValue3 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 4.
        /// </summary>
        public string? DepartmentDefinedFieldValue4 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 5.
        /// </summary>
        public string? DepartmentDefinedFieldValue5 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 6.
        /// </summary>
        public string? DepartmentDefinedFieldValue6 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 7.
        /// </summary>
        public string? DepartmentDefinedFieldValue7 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 8.
        /// </summary>
        public string? DepartmentDefinedFieldValue8 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 9.
        /// </summary>
        public string? DepartmentDefinedFieldValue9 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 10.
        /// </summary>
        public string? DepartmentDefinedFieldValue10 { get; set; }

        /// <summary>
        /// Gets or sets the fiscal year.
        /// </summary>
        public int? FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets the project number.
        /// </summary>
        public string? ProjectNumber { get; set; }

        /// <summary>
        /// Gets or sets whether this request is exempt from hour-based policy rules.
        /// </summary>
        public bool IsHoursPolicyExempt { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this request is a premium paid event.
        /// </summary>
        public bool IsPremiumPaidEvent { get; set; } = false;

        /// <summary>
        /// Gets or sets whether department fees apply to this request.
        /// </summary>
        public bool EnableFees { get; set; } = true;

        /// <summary>
        /// Selected department fee IDs for this request. When null on update, existing selections are unchanged.
        /// </summary>
        public List<Guid>? SelectedDepartmentFeeIds { get; set; }

        /// <summary>
        /// When true, confirms updating future assignment pay rates when <see cref="IsPremiumPaidEvent"/> changes.
        /// </summary>
        public bool ConfirmConflict { get; set; }

        public RequestTypeDto? RequestType { get; set; }

        /// <summary>
        /// Post positions selected on the date-change popup to re-create on the new request date.
        /// All other assigned post positions on the prior date are cancelled without transfer.
        /// </summary>
        public List<Guid>? TransferWithConflictPostPositionIds { get; set; }

        private string? _primaryPhone;
        private string? _cellPhone;
        private string? _secondaryPhone;
        private string? _alternateCellPhone;

        /// <summary>
        /// Gets or sets whether the request has been updated.
        /// </summary>
        public bool? RequestHasBeenUpdated { get; set; }

        /// <summary>
        /// Gets or sets the count of related records (e.g. PostCount) for this request.
        /// </summary>
        public long PostCount { get; set; }

        public List<PostDto> Posts { get; set; }
        public List<RequestScheduleDto>? RequestSchedules { get; set; }
        public RequesterDto? Requester { get; set; }

        public void PrepareForCloneAsNewRequestSeed()
        {
            RequestID = Guid.Empty;
            Requester = null;
            RequestType = null;
            Posts = new List<PostDto>();
            RequestSchedules = new List<RequestScheduleDto>();
        }

        public void TrimPostByPostId(Guid postId)
        {
            if (Posts == null || Posts.Count == 0)
                return;

            Posts = Posts
                .Where(p => p.PostID == postId)
                .ToList();
        }
        public ReleaseAs GetReleaseAsByDate(DateOnly date)
        {
            if (RequestSchedules == null || !RequestSchedules.Any())
                return ReleaseAs;
            return RequestSchedules
            .Where(rs =>
                (rs.StartDate == null || date >= rs.StartDate) &&
                (rs.EndDate == null || date <= rs.EndDate))   // filter valid
            .OrderByDescending(rs => rs.StartDate)            // latest wins
            .Select(rs => rs.GetReleaseAsStatusByDate(date))  // use your function
            .FirstOrDefault();
        }

        public RequestStatus GetRequestStatusByDate(DateOnly date)
        {
            if (RequestSchedules == null || !RequestSchedules.Any())
                return RequestStatus.Pending;
            return RequestSchedules
            .Where(rs =>
                (rs.StartDate == null || date >= rs.StartDate) &&
                (rs.EndDate == null || date <= rs.EndDate))   // filter valid
            .OrderByDescending(rs => rs.StartDate)            // latest wins
            .Select(rs => rs.GeRequestStatusByDate(date))  // use your function
            .FirstOrDefault();
        }
        /// <summary>
        /// Returns whether the effective release for <paramref name="date"/> equals <paramref name="release"/>
        /// (same value as <see cref="GetReleaseAsByDate"/> compared to <paramref name="release"/>).
        /// </summary>
        public bool IsReleaseAsForDate(DateOnly date, ReleaseAs release)
            => GetReleaseAsByDate(date) == release;

        public DateOnly? GetReleaseNowScheduleStartDate(DateOnly date) =>
            RequestSchedules?
                .Select(rs => rs.GetJobsPostedScheduleStartDate(date))
                .Where(start => start.HasValue)
                .MinBy(start => start!.Value);

        public int GetTotalPostPositionsCount()
        {
            if (Posts == null || !Posts.Any())
                return 0;

            return Posts.Sum(p => p.TotalPostPositions);
        }
        public bool CheckEmployeeReleaseStatusById(Guid employeeId, ReleaseAs releaseAs)
        {
            if (releaseAs == ReleaseAs.Open)
                return true;
            if (!CanEmployeeViewRequest(employeeId))
                return false;

            bool isManager = Requester?.IsEmployeeManager(employeeId) == true;

            if (isManager)
                return true;

            if (Requester == null || Requester.RequesterManagers == null || !Requester.RequesterManagers.Any())
                return true;

            return releaseAs switch
            {
                ReleaseAs.Preferred =>
                    Requester.RequesterManagers.Any(m => m.IsEmployeeEligibleForReleaseAs(employeeId, ReleaseAs.Preferred)),

                ReleaseAs.TenureBased =>
                    RequesterManagerID == null
                        ? true
                        : Requester.RequesterManagers.Any(m => m.IsEmployeeEligibleForReleaseAs(employeeId, ReleaseAs.TenureBased)),

                ReleaseAs.Managed =>
                    Requester.RequesterManagers.Any(m => m.IsEmployeeEligibleForReleaseAs(employeeId, ReleaseAs.Managed)),

                _ => false
            };

        }
        public bool CanEmployeeViewRequest(Guid employeeId)
        {
            // 🟢 Admin-created request → visible to all
            if (RequesterManagerID == null)
                return true;

            if (Requester?.RequesterManagers == null || !Requester.RequesterManagers.Any())
                return true;

            var creatorManager = Requester.GetManager(RequesterManagerID.Value);

            if (creatorManager == null)
                return true;

            // 🔹 Employee belongs to creator manager (manager OR employee)
            bool isUnderCreator = Requester.IsEmployeeUnderCreatorScope(creatorManager.RequesterManagerID!.Value, employeeId);

            if (isUnderCreator)
                return true;

            // 🔒 Not shared → nobody else allowed
            if (!creatorManager.IsRequestShared)
                return false;

            // 🌐 Shared → visible to employees under OTHER managers
            return Requester.RequesterManagers?
                     .Where(m => m.RequesterManagerID != creatorManager.RequesterManagerID)
                     .Any(m => m.ContainsEmployee(employeeId)) == true;
        }
        public List<DateOnly> RemoveRestrictedEmployeeForThisRequester(List<DateOnly> dates, List<RequesterRestrictedListDto>? restrictions)
        {
            if (RequesterID == null ||
                restrictions == null ||
                !restrictions.Any())
                return dates;

            var validDates = new List<DateOnly>();

            foreach (var date in dates)
            {
                bool isRestricted = restrictions.Any(r =>
                    r.RequesterID == RequesterID &&
                    r.EffectiveStartDate <= date &&
                    r.EffectiveEndDate >= date);

                if (!isRestricted)
                    validDates.Add(date);
            }

            return validDates;
        }

        public AvailablePostResultDto GetAvailablePosts(EmployeeDto employee, 
            DateTime startDateTime, 
            DateTime currentDateTime,
            DateOnly endDate, 
            ReleaseAs releaseAs, 
            List<PostPositionDto>? employeeCurrentPostPosition = null,
            List<WaitingPostPositionDto>? employeeCurrentWaitingPostPosition = null, 
            List<RequesterRestrictedListDto>? restrictedEmployessRequesterId = null, 
            DepartmentPolicyDto? departmentPolicy = null, 
            EmployeePolicyUsageDto? employeePolicyUsage = null, 
            bool displayWaitingList = false,
            bool isDepartmentAdmin = false, 
            bool validatePost = true, 
            Guid? postPositionId = null, 
            bool isBulkAssign = false, 
            bool browseOnlyTimeConflict = true,
            bool isPlaceHolder=false,
            string placeHolder = null)
        {
            var result = new AvailablePostResultDto();
            var startDateOnly = DateOnly.FromDateTime(startDateTime); // ✅ derive date
            bool isPolicyEnabled = departmentPolicy != null  && departmentPolicy.EnablePolicyManagement && employeePolicyUsage != null && !isPlaceHolder;
            string employeeName = isPlaceHolder ? placeHolder : employee.GetFullName();
                
            if (!isPlaceHolder && employee != null && DepartmentID != employee.DepartmentID)
            {
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_EMPLOYEE_DIFFERENT_DEPARTMENT_FOR_EMPLOYEE, employeeName);
                return result;
            }

            if (isPolicyEnabled && !isBulkAssign && employeePolicyUsage.HasOutstandingReadyToClosePost(departmentPolicy))
            {
                return new AvailablePostResultDto
                {
                    Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_OUTSTANDING_READY_TO_CLOSE_JOB),
                    HasOutstandingReadyToClosePost = true
                };
            }

            if (isPolicyEnabled && employeePolicyUsage.HasAlreadyAppliedMaxPostToday(departmentPolicy))
            {
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_MAX_JOBS_APPLIED_TODAY);
                return result;
            }

            if (!isPlaceHolder && !isDepartmentAdmin && !CheckEmployeeReleaseStatusById(employee.EmployeeID.Value, releaseAs))
            {
                result.Message = ModelValidationErrorCodes.GetErrorMessage(
                    ModelValidationErrorCodes.REQUEST_RELEASE_CRITERIA_NOT_MET,
                    employeeName,
                    Title);
                return result;
            }

            var dates = CollectDates(startDateOnly, endDate,isBulkAssign);
            if (!dates.Any())
            {
                result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_NO_VALID_DATES_FOUND);
                return result;
            }
            if (!isPlaceHolder && employee != null)
            {
                dates = employee.RemoveSuspendedDates(dates);
                if (!dates.Any())
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(ModelValidationErrorCodes.REQUEST_SUSPENDED_EMPLOYEE_NOT_ALLOWED_TO_SCHEDULE_FOR_EMPLOYEE, employeeName);
                    return result;
                }
            }

            var allPosts = new List<AvailablePostDto>();
            string? policyRejectionReason = null;

            foreach (var post in Posts)
            {
                // remove blocked
                var workingDates = post.GetUnblockedDates(dates);

                if (!workingDates.Any())
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.REQUEST_JOB_ID_BLOCKED,
                        post.PostTitle);
                    continue;
                }
                var validDates = isPlaceHolder ? workingDates : RemoveRestrictedEmployeeForThisRequester(workingDates, restrictedEmployessRequesterId); if (!validDates.Any())
                {
                    result.Message = ModelValidationErrorCodes.GetErrorMessage(
                        ModelValidationErrorCodes.REQUEST_EMPLOYEE_RESTRICTED_BY_REQUESTER,
                        employeeName);
                    continue;
                }

                var policyAllowedDates = new List<DateOnly>();

                foreach (var date in validDates)
                {
                    if (!isPlaceHolder && isPolicyEnabled && !employeePolicyUsage.CheckDepartmentPolicy(departmentPolicy, date, IsHoursPolicyExempt, out var rejectionReason))
                    {
                        // capture first rejection reason only
                        policyRejectionReason ??= rejectionReason;
                        continue;
                    }
                    policyAllowedDates.Add(date);
                }

                if (!policyAllowedDates.Any())
                {
                    // nothing allowed for this post
                    result.Message ??= policyRejectionReason;
                    continue;
                }
                bool shouldValidatePost = validatePost || !IsSpecialEvent.GetValueOrDefault();
                AvailablePostResultDto postResult = new AvailablePostResultDto();
                postResult = post.GetAvailablePostsForDates( employee, policyAllowedDates, currentDateTime, displayWaitingList, employeeCurrentPostPosition, employeeCurrentWaitingPostPosition, departmentPolicy, postPositionId, Posts, shouldValidatePost, browseOnlyTimeConflict,isPlaceHolder,placeHolder);

                if (postResult.Posts != null)
                    allPosts.AddRange(postResult.Posts);
                 result.Message = postResult.Message;
                // Copy the conflict confirmation flag if set
                if (postResult.HasTimeConflictRequiringConfirmation)
                    result.HasTimeConflictRequiringConfirmation = true;
                if (postResult.HasCancellationPendingSlotsRequiringDialog)
                    result.HasCancellationPendingSlotsRequiringDialog = true;
                if (postResult.HasNoSlot)
                    result.HasNoSlot = true;
            }

            result.Posts = allPosts;
            return result;
        }

        public List<DateOnly> CollectDates(DateOnly searchStart, DateOnly searchEnd,bool isBulkAssign = false)
        {
            var dates = new List<DateOnly>();
            var matchingRecurrence = RequestType;
            var releasedScheduleWindows = RequestSchedules?
                .Where(s =>
                    isBulkAssign ? true: s.RequestStatus == RequestStatus.ReleaseNow &&
                    s.StartDate.HasValue &&
                    s.EndDate.HasValue)
                .Select(s => (Start: s.StartDate!.Value, End: s.EndDate!.Value))
                .Where(w => w.Start <= searchEnd && w.End >= searchStart)
                .ToList() ?? new List<(DateOnly Start, DateOnly End)>();

            // Use released RequestSchedule windows when available;
            // otherwise keep backward-compatible Request-level date behavior.
            var windows = releasedScheduleWindows.Any()
                ? releasedScheduleWindows
                : new List<(DateOnly Start, DateOnly End)> { };

            // 1. One - Time(no recurrence object OR RequestType == OneTime)
            if (matchingRecurrence == null || matchingRecurrence.RequestType == RequestTypes.OneTime)
            {
                foreach (var window in windows)
                {
                    if (window.Start >= searchStart && window.Start <= searchEnd)
                    {
                        dates.Add(window.Start);
                    }
                }
                return dates.Distinct().OrderBy(d => d).ToList();
            }

            // 2. Delegate to recurrence logic
            foreach (var window in windows)
            {
                var effectiveSearchStart = window.Start > searchStart ? window.Start : searchStart;
                var effectiveSearchEnd = window.End < searchEnd ? window.End : searchEnd;

                if (effectiveSearchStart > effectiveSearchEnd)
                    continue;

                switch (matchingRecurrence.RequestType)
                {
                    case RequestTypes.Daily:
                        CollectDailyDates(window.Start, window.End, matchingRecurrence, dates, effectiveSearchStart, effectiveSearchEnd);
                        break;
                    case RequestTypes.Weekly:
                        CollectWeeklyDates(window.Start, window.End, matchingRecurrence, dates, effectiveSearchStart, effectiveSearchEnd);
                        break;
                    case RequestTypes.Monthly:
                        CollectMonthlyDates(window.Start, window.End, matchingRecurrence, dates, effectiveSearchStart, effectiveSearchEnd);
                        break;
                }
            }

            return dates.Distinct().OrderBy(d => d).ToList();
        }

        private static void CollectDailyDates(DateOnly start, DateOnly end, RequestTypeDto recurrence, List<DateOnly> dates, DateOnly searchStart, DateOnly searchEnd)
        {
            int interval = recurrence.RepeatEvery;

            if (interval < 1) interval = 1;

            int daysDiff = searchStart.DayNumber - start.DayNumber;
            int k = 0;
            if (daysDiff > 0)
            {
                k = (int)Math.Ceiling((double)daysDiff / interval);
            }

            if (k < 0) k = 0;

            DateOnly current = start.AddDays(k * interval);

            while (current <= end && current <= searchEnd)
            {
                if (current >= searchStart)
                {
                    dates.Add(current);
                }
                current = current.AddDays(interval);
            }
        }

        private static void CollectWeeklyDates(DateOnly start, DateOnly end, RequestTypeDto recurrence, List<DateOnly> dates, DateOnly searchStart, DateOnly searchEnd)
        {
            int interval = recurrence.RepeatEvery;
            if (interval < 1) interval = 1;

            var validDays = new HashSet<DayOfWeek>();

            if (recurrence.DaysOfWeek != null)
            {
                foreach (var dayInt in recurrence.DaysOfWeek)
                {
                    if (Enum.IsDefined(typeof(DayOfWeek), dayInt))
                    {
                        validDays.Add((DayOfWeek)dayInt);
                    }
                }
            }

            if (!validDays.Any()) return;

            DateOnly firstWeekStart = start.AddDays(-(int)start.DayOfWeek);

            DateOnly currentWeekStart = firstWeekStart;

            if (searchStart > currentWeekStart)
            {
                int daysSinceStart = searchStart.DayNumber - currentWeekStart.DayNumber;
                int weeksSinceStart = daysSinceStart / 7;
                int intervalsSince = weeksSinceStart / interval;
                int weeksToSkip = intervalsSince * interval;

                currentWeekStart = currentWeekStart.AddDays(weeksToSkip * 7);
            }

            while (currentWeekStart <= end && currentWeekStart <= searchEnd)
            {
                foreach (var dayOfWeek in validDays)
                {
                    DateOnly potentialDate = currentWeekStart.AddDays((int)dayOfWeek);

                    if (potentialDate < start) continue;
                    if (potentialDate > end) continue;

                    if (potentialDate >= searchStart && potentialDate <= searchEnd)
                    {
                        dates.Add(potentialDate);
                    }
                }

                currentWeekStart = currentWeekStart.AddDays(7 * interval);
            }
        }

        private static void CollectMonthlyDates(DateOnly start, DateOnly end, RequestTypeDto recurrence, List<DateOnly> dates, DateOnly searchStart, DateOnly searchEnd)
        {
            int interval = recurrence.RepeatEvery;
            if (interval < 1) interval = 1;

            DateOnly currentMonth = new DateOnly(start.Year, start.Month, 1);

            if (searchStart > currentMonth)
            {
                int monthsDiff = ((searchStart.Year - currentMonth.Year) * 12) + searchStart.Month - currentMonth.Month;
                int intervalsDiff = monthsDiff / interval;
                currentMonth = currentMonth.AddMonths(intervalsDiff * interval);
            }

            while (currentMonth <= end && currentMonth <= searchEnd)
            {
                DateOnly? dateToAdd = null;

                if (recurrence.MonthlyMode == MonthlyMode.ByDate)
                {
                    if (recurrence.DaysOfMonth != null && recurrence.DaysOfMonth.Any())
                    {
                        foreach (var day in recurrence.DaysOfMonth)
                        {
                            int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
                            if (day <= daysInMonth)
                            {
                                dateToAdd = new DateOnly(currentMonth.Year, currentMonth.Month, day);

                                if (dateToAdd.HasValue)
                                {
                                    DateOnly d = dateToAdd.Value;
                                    if (d >= start && d <= end && d >= searchStart && d <= searchEnd)
                                    {
                                        dates.Add(d);
                                    }
                                }
                            }
                        }
                        dateToAdd = null;
                    }
                }
                else if (recurrence.MonthlyMode == MonthlyMode.ByPattern)
                {
                    if (recurrence.WeekIndex.HasValue && recurrence.DaysOfWeek != null && recurrence.DaysOfWeek.Any())
                    {
                        foreach (var patternDayInt in recurrence.DaysOfWeek)
                        {
                            if (!Enum.IsDefined(typeof(DayOfWeek), patternDayInt))
                                continue;

                            var dayOfWeek = (DayOfWeek)patternDayInt;
                            // WeekIndex 0 is every matching weekday in the month.
                            if (recurrence.WeekIndex.Value == 0)
                            {
                                foreach (var occurrence in GetAllWeekdaysOfMonth(currentMonth.Year, currentMonth.Month, dayOfWeek))
                                {
                                    if (occurrence >= start && occurrence <= end && occurrence >= searchStart && occurrence <= searchEnd)
                                        dates.Add(occurrence);
                                }
                                continue;
                            }

                            var d = GetNthWeekdayOfMonth(currentMonth.Year, currentMonth.Month,
                                recurrence.WeekIndex.Value,
                                dayOfWeek);

                            if (d.HasValue && d.Value >= start && d.Value <= end && d.Value >= searchStart && d.Value <= searchEnd)
                            {
                                dates.Add(d.Value);
                            }
                        }
                    }
                }

                currentMonth = currentMonth.AddMonths(interval);
            }
        }

        /// <summary>
        /// Schedule generation only. Day 31 in a shorter month becomes that month's last day.
        /// </summary>
        public static DateOnly GetScheduleDayWithinMonth(int year, int month, int day)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var effectiveDay = day < 1 ? 1 : Math.Min(day, daysInMonth);
            return new DateOnly(year, month, effectiveDay);
        }

        private static IEnumerable<DateOnly> GetAllWeekdaysOfMonth(int year, int month, DayOfWeek dayOfWeek)
        {
            var first = GetNthWeekdayOfMonth(year, month, 1, dayOfWeek);
            if (!first.HasValue)
                yield break;

            var daysInMonth = DateTime.DaysInMonth(year, month);
            for (var day = first.Value; day.Month == month && day.Day <= daysInMonth; day = day.AddDays(7))
                yield return day;
        }

        private static DateOnly? GetNthWeekdayOfMonth(int year, int month, int n, DayOfWeek dayOfWeek)
        {
            if (n < 1 || n > 5) return null;

            DateOnly firstOfMonth = new DateOnly(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            int delta = (int)dayOfWeek - (int)firstOfMonth.DayOfWeek;
            if (delta < 0) delta += 7;

            int firstDay = 1 + delta;

            if (n < 5)
            {
                int targetDay = firstDay + (n - 1) * 7;
                if (targetDay > daysInMonth) return null;

                return new DateOnly(year, month, targetDay);
            }
            else
            {
                DateOnly lastOfMonth = new DateOnly(year, month, daysInMonth);
                int deltaEnd = (int)lastOfMonth.DayOfWeek - (int)dayOfWeek;
                if (deltaEnd < 0) deltaEnd += 7;

                return lastOfMonth.AddDays(-deltaEnd);
            }
        }
        public RosterResponseDto BuildRosterResponse(
            DateOnly dateOfWork,
            Dictionary<Guid, EmployeeDto> employees,
            IReadOnlyDictionary<Guid, IReadOnlyDictionary<Guid, string?>> vehicleNameByPostAndEmployee)
        {
            List<RosterInformationDto> rosterRows = [];

            foreach (var post in Posts ?? [])
            {
                vehicleNameByPostAndEmployee.TryGetValue(post.PostID ?? Guid.Empty, out var vehicleNameByEmployeeId);
                rosterRows.AddRange(post.CreateRosterRowsForPost(dateOfWork, employees, vehicleNameByEmployeeId));
            }

            var rosterTables = new RosterTableDto
            {
                Rows = rosterRows
            };

            return new RosterResponseDto
            {
                RequesterName = Requester?.Name ?? string.Empty,
                RosterDate = DateHelper.ToRosterDisplayDate(dateOfWork),
                Tables = rosterTables.CreateRosterTablesFromRows()
            };

        }
    }
}

