using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Closed-post audit list row: audit trail fields plus current job snapshot from the main database.
    /// </summary>
    public class ClosedPostPositionAuditListDto
    {
        public long AuditId { get; set; }

        public Guid ClosedPostPositionId { get; set; }

        public string? EmployeeName { get; set; }

        public DateOnly? Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        /// <summary>Hours worked display (e.g. "6 hr 45 min").</summary>
        public string HoursWorked { get; set; } = string.Empty;

        /// <summary>Hours required display (e.g. "7 hr").</summary>
        public string HoursRequired { get; set; } = string.Empty;

        public string? JobTitle { get; set; }

        /// <summary>Closed-post status label (e.g. "Closed").</summary>
        public string Type { get; set; } = nameof(ClosedPostPositionStatus.Closed);

        public AuditAction Action { get; set; }

        public DateTime ChangedOn { get; set; }

        /// <summary>JSON field-level changes for the view modal.</summary>
        public string ChangedFields { get; set; } = "[]";
    }
}
