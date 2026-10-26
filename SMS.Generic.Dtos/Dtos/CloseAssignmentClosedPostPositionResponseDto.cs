using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Close-assignments list item: closed post position row with assignment context.
    /// </summary>
    public class CloseAssignmentClosedPostPositionResponseDto
    {
        public Guid ClosedPostPositionID { get; set; }

        /// <summary>Self-reference when this row is linked to a prior closed post position (split/edit chain).</summary>
        public Guid? ParentClosedPostPositionID { get; set; }

        public Guid PostPositionID { get; set; }
        public Guid PostID { get; set; }

        /// <summary>Assigned employee (from <see cref="PostPosition"/>).</summary>
        public Guid? EmployeeID { get; set; }

        /// <summary>From <c>PostPosition.EnableInvoiceAndPayroll</c>.</summary>
        public bool EnableInvoiceAndPayroll { get; set; }

        /// <summary>Convenience from related post (for search and display).</summary>
        public string? PostTitle { get; set; }

        /// <summary>Job-level requester id from <c>Request.RequesterID</c>.</summary>
        public Guid? RequesterID { get; set; }

        /// <summary>Requester display name from <c>Requester.Name</c>.</summary>
        public string? RequesterTitle { get; set; }

        /// <summary>Scheduled job length in minutes from <c>Post.PostLength</c>.</summary>
        public int PostLength { get; set; }

        /// <summary>Work date of the post position assignment; used for date range and ordering.</summary>
        public DateOnly? DateOfWork { get; set; }

        /// <summary>From <c>PostPosition.WasUnclosed</c> for the linked assignment.</summary>
        public bool WasUnclosed { get; set; }
        public bool IsEdited { get; set; }
        public ClosedPostPositionStatus? Status { get; set; }

        public DateOnly? WorkStartDate { get; set; }
        public TimeOnly? WorkStartTime { get; set; }
        public TimeOnly? WorkEndTime { get; set; }
        public int HoursWorked { get; set; }
        public string? CloseBy { get; set; }
    }
}
