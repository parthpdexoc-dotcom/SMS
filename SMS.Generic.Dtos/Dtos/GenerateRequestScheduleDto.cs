using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Payload for generating request schedules from the request type configuration.
    /// </summary>
    public class GenerateRequestScheduleDto
    {
        [Required]
        public Guid RequestId { get; set; }

        /// <summary>
        /// Optional absolute schedule date override. When omitted, Released On is derived from the
        /// selected weekday / month-day configuration (weekly: one week earlier; monthly: one month
        /// earlier on the same by-date or by-pattern rule).
        /// </summary>
        public DateOnly? ReleaseBeforeDate { get; set; }

        /// <summary>Weekly (and monthly-by-pattern) selected weekdays. 0=Sunday … 6=Saturday.</summary>
        public List<int>? DaysOfWeek { get; set; }

        /// <summary>Monthly by-date day(s) of month (1–31).</summary>
        public List<int>? DaysOfMonth { get; set; }

        public MonthlyMode? MonthlyMode { get; set; }

        /// <summary>Monthly by-pattern week index (1=First … 5=Last).</summary>
        public int? WeekIndex { get; set; }

        /// <summary>
        /// Scheduled release time applied to each generated schedule. When omitted, midnight is used
        /// so UpdateReleasedOnToReleased can flip Released On → Release Now.
        /// </summary>
        public TimeOnly? ScheduledTime { get; set; }

        /// <summary>
        /// Tenure Available for hours (1, 24, 48, or 72) added to each schedule date and time
        /// to set Process/Offer Date and Process Time. Custom is not used.
        /// </summary>
        public int? ReleaseHours { get; set; }

        /// <summary>Sent only when the request end date changed.</summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>Sent only when the request type changed.</summary>
        public RequestTypes? RequestType { get; set; }
    }
}
