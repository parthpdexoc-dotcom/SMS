using SMS.Generic.Enums;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequestType (formerly RequestRecurrence).
    /// </summary>
    public class RequestTypeDto
    {
        public Guid RequestTypeID { get; set; }
        public Guid RequestID { get; set; }

        /// <summary>
        /// Formerly RecurrenceUnit.
        /// </summary>
        public RequestTypes RequestType { get; set; }

        /// <summary>
        /// Formerly Interval.
        /// </summary>
        public int RepeatEvery { get; set; }

        /// <summary>
        /// Formerly WeekDays.
        /// </summary>
        public List<int>? DaysOfWeek { get; set; }

        /// <summary>
        /// Formerly MonthDays.
        /// </summary>
        public List<int>? DaysOfMonth { get; set; }

        public MonthlyMode? MonthlyMode { get; set; }
        public int? WeekIndex { get; set; }

        public void PrepareForCloneAsNewRequest(Guid newRequestId)
        {
            RequestTypeID = Guid.Empty;
            RequestID = newRequestId;
        }

    }
}
