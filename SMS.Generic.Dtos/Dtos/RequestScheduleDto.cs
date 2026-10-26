using System;
using System.ComponentModel.DataAnnotations;
using SMS.Generic.Common.Enums;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    public class RequestScheduleDto : BaseDto
    {
        public Guid RequestScheduleID { get; set; }

        [Required]
        public Guid RequestId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public ReleaseAs ReleaseAs { get; set; }

        public int? ReleaseHours { get; set; }

        public RequestStatus RequestStatus { get; set; }

        public DateOnly? ScheduleDate { get; set; }

        public TimeOnly? ScheduledTime { get; set; }

        public DateOnly? ProcessDate { get; set; }

        public TimeOnly? ProcessTime { get; set; }


        public DateTime? ProcessedAt { get; set; }

        public ProcessingStatus ProcessingStatus { get; set; } = ProcessingStatus.Pending;

        [MaxLength(2000)]
        public string? ProcessingResult { get; set; }

        public Guid? ProcessingJobId { get; set; }

        /// <summary>
        /// When true, auto-generate and request changes do not modify this schedule row.
        /// </summary>
        public bool PreserveOnRegenerate { get; set; }

        /// <summary>
        /// Request-only flag from UI: when true on save after clone, sends jobs-posted emails for all posts under this request.
        /// </summary>
        public bool IsCloned { get; set; }

        /// <summary>
        /// Number of schedule rows created by Generate Schedule.
        /// </summary>
        public int? GeneratedScheduleCount { get; set; }

        /// <summary>
        /// Tenure generate notice. Not stored. Set when tenure rows were skipped because the process date is the work day.
        /// </summary>
        public string? GenerationWarning { get; set; }

        public void PrepareForCloneAsNewRequest(Guid newRequestId)
        {
            RequestScheduleID = Guid.Empty;
            RequestId = newRequestId;

            // cloned request should start with fresh processing state
            ProcessedAt = null;
            ProcessingJobId = null;
            ProcessingResult = null;
            ProcessingStatus = ProcessingStatus.Pending;
        }

        public ReleaseAs GetReleaseAsStatusByDate(DateOnly date)
        {
            bool isApplicable = (StartDate == null || date >= StartDate) && (EndDate == null || date <= EndDate);
            return isApplicable ? ReleaseAs : default;
        }
        public RequestStatus GeRequestStatusByDate(DateOnly date)
        {
            bool isApplicable = (StartDate == null || date >= StartDate) && (EndDate == null || date <= EndDate);
            return isApplicable ? RequestStatus : default;
        }

        public bool IsEligibleForJobsPostedEmail(DateOnly date) =>
            RequestStatus == RequestStatus.ReleaseNow &&
            ((StartDate is DateOnly start && start >= date) ||
             (EndDate is DateOnly end && end >= date));

        public DateOnly? GetJobsPostedScheduleStartDate(DateOnly date) =>
            StartDate is DateOnly start && IsEligibleForJobsPostedEmail(date)
                ? start
                : null;

        public bool Overlaps(DateOnly? start, DateOnly? end) =>
            (start is not null || end is not null)
            && (start ?? DateOnly.MinValue) <= (EndDate ?? DateOnly.MaxValue)
            && (StartDate ?? DateOnly.MinValue) <= (end ?? DateOnly.MaxValue);

        public static RequestScheduleDto CreateGenerated(
            Guid requestId,
            DateOnly start,
            DateOnly end,
            ReleaseAs releaseAs,
            DateOnly releaseBeforeDate,
            TimeOnly scheduledTime,
            int? releaseHours = null)
        {
            DateOnly? processDate = null;
            TimeOnly? processTime = null;
            if (releaseHours is int hours)
            {
                var processAt = releaseBeforeDate.ToDateTime(scheduledTime).AddHours(hours);
                processDate = DateOnly.FromDateTime(processAt);
                processTime = TimeOnly.FromDateTime(processAt);
            }

            return new()
            {
                RequestScheduleID = Guid.Empty,
                RequestId = requestId,
                StartDate = start,
                EndDate = end,
                ReleaseAs = releaseAs,
                RequestStatus = RequestStatus.ReleasedOn,
                ScheduleDate = releaseBeforeDate,
                ScheduledTime = scheduledTime,
                ReleaseHours = releaseHours,
                ProcessDate = processDate,
                ProcessTime = processTime,
                ProcessingStatus = ProcessingStatus.Pending,
                IsCloned = false
            };
        }
    }
    public class RequestScheduleProcessingResultDto
    {
        public Guid RequestId { get; set; }

        public int TotalEmployees { get; set; }
        public int AssignedCount { get; set; }
        public int FailedCount { get; set; }

        public List<EmployeeResultDto> EmployeeResults { get; set; } = new();

        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        // ✅ Constructor
        public RequestScheduleProcessingResultDto(Guid requestId)
        {
            RequestId = requestId;
            EmployeeResults = new List<EmployeeResultDto>();
            IsSuccess = true;
        }

        // ✅ Add Success
        public void AddSuccess(Guid employeeId)
        {
            EmployeeResults.Add(EmployeeResultDto.Success(employeeId));
        }

        // ✅ Add Failure
        public void AddFailure(Guid employeeId, string? reason)
        {
            EmployeeResults.Add(EmployeeResultDto.Failure(employeeId, reason));
        }

        // ✅ Mark whole request failed
        public void MarkFailed(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }
        // ✅ Mark whole request failed
        public void MarkSuccessed(string successsMessage)
        {
            IsSuccess = true;
            SuccessMessage = successsMessage;
        }
        // ✅ Finalize counts (call once at end)
        public void FinalizeResult()
        {
            TotalEmployees = EmployeeResults.Count;
            AssignedCount = EmployeeResults.Count(x => x.IsAssigned);
            FailedCount = EmployeeResults.Count(x => !x.IsAssigned);
        }
    }

    public class EmployeeResultDto
    {
        public Guid EmployeeId { get; set; }
        public bool IsAssigned { get; set; }
        public string? Reason { get; set; }
        // 🔒 Private constructor
        private EmployeeResultDto(Guid employeeId, bool isAssigned, string? reason)
        {
            EmployeeId = employeeId;
            IsAssigned = isAssigned;
            Reason = reason;
        }

        // ✅ Success factory
        public static EmployeeResultDto Success(Guid employeeId)
        {
            return new EmployeeResultDto(employeeId, true, null);
        }

        // ✅ Failure factory
        public static EmployeeResultDto Failure(Guid employeeId, string? reason)
        {
            return new EmployeeResultDto(employeeId, false, reason);
        }
    }
    public class RequestScheduleProcessDueRequestDto
    {
        public Guid RequestID { get; set; }

        /// <summary>Request title for logging and job-run audit display.</summary>
        public string RequestTitle { get; set; } = string.Empty;

        public List<Guid> PostIDs { get; set; } = new();
        public Guid DepartmentID { get; set; }
        public string TimeZoneId { get; set; } = "UTC";

        /// <summary>
        /// The department-local scheduled processing time (to the minute).
        /// </summary>
        public DateTime ProcessLocalDateTime { get; set; }

        /// <summary>
        /// UTC time when the scheduler evaluated the request as due (to the minute).
        /// </summary>
        public DateTime EvaluatedAtUtc { get; set; }
    }

    /// <summary>
    /// Result of bulk <see cref="RequestSchedule"/> status updates (e.g. ReleasedOn scheduled job).
    /// Used for logging and for <c>UpdateReleasedOnToReleased_Audit</c> job summaries.
    /// </summary>
    public sealed class RequestScheduleBulkUpdateResult
    {
        /// <summary>Number of <c>RequestSchedule</c> rows written in this operation.</summary>
        public int RowCount { get; init; }

        /// <summary>Distinct parent requests for those rows (title/department for audit display).</summary>
        public IReadOnlyList<RequestScheduleAffectedRequest> AffectedRequests { get; init; } =
            Array.Empty<RequestScheduleAffectedRequest>();
    }

    /// <summary>One parent request referenced by schedules updated in a bulk operation.</summary>
    public sealed class RequestScheduleAffectedRequest
    {
        public Guid RequestId { get; init; }

        public string RequestTitle { get; init; } = string.Empty;

        public Guid? DepartmentId { get; init; }
    }
}
