using SMS.Generic.Common.Helpers;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Lightweight review data for one-time request date changes (assigned rows + employee slots on the new date).
    /// </summary>
    public class OneTimeDateChangeReviewDataDto
    {
        public List<OneTimeDateChangeAssignedRowDto> AssignedOnOldDate { get; set; } = new();

        public List<OneTimeDateChangeConflictSlotRowDto> EmployeeSlotsOnNewDate { get; set; } = new();
    }

    /// <summary>
    /// Assigned employee row on the prior request date.
    /// </summary>
    public class OneTimeDateChangeAssignedRowDto
    {
        public Guid PostPositionID { get; set; }

        public Guid EmployeeID { get; set; }

        public string? EmployeeFirstName { get; set; }

        public string? EmployeeLastName { get; set; }

        public Guid PostID { get; set; }

        public string? PostTitle { get; set; }

        public DateOnly DateOfWork { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int Duration { get; set; }
    }

    /// <summary>
    /// Employee job slot on the proposed new date used for time-overlap checks.
    /// </summary>
    public class OneTimeDateChangeConflictSlotRowDto
    {
        public Guid EmployeeID { get; set; }

        public Guid PostID { get; set; }

        public DateOnly DateOfWork { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int LengthOfWork { get; set; }
    }

    /// <summary>
    /// 409 conflict payload when a one-time request start date changes while employees remain assigned on the prior date.
    /// </summary>
    public class RequestOneTimeDateChangeConflictDto
    {
        public string Message { get; set; } = string.Empty;

        public List<RequestOneTimeDateChangeAssignmentDto> AssignedPostPositions { get; set; } = new();
    }

    /// <summary>
    /// Assigned post position on the prior request date, with per-row time conflict on the proposed new date.
    /// </summary>
    public class RequestOneTimeDateChangeAssignmentDto
    {
        public RequestOneTimeDateChangeAssignmentDto()
        {
        }

        public RequestOneTimeDateChangeAssignmentDto(
            OneTimeDateChangeAssignedRowDto row,
            bool hasTimeConflict)
        {
            PostPositionID = row.PostPositionID;
            EmployeeID = row.EmployeeID;
            EmployeeName = PersonNameFormatHelper.FormatLastNameCommaFirstName(row.EmployeeLastName, row.EmployeeFirstName) ?? string.Empty;
            PostTitle = row.PostTitle;
            DateOfWork = row.DateOfWork;
            StartTime = row.StartTime;
            EndTime = row.EndTime;
            Duration = row.Duration;
            HasTimeConflict = hasTimeConflict;
        }

        public Guid PostPositionID { get; set; }

        public Guid EmployeeID { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string? PostTitle { get; set; }

        public DateOnly DateOfWork { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int Duration { get; set; }

        public bool HasTimeConflict { get; set; }
    }
}
