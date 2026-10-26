using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Post position with assigned employee details and GPS coordinates array.
    /// </summary>
    public class PostPositionEmployeeLocationDto
    {
        public Guid PostPositionId { get; set; }
        public DateOnly? DateOfWork { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public PostPositionStatus Status { get; set; }
        public Guid? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? BadgeNumber { get; set; }
        public string? EmployeeType { get; set; }
        public string? CallSign { get; set; }
        public bool IsPlaceholder { get; set; }

        /// <summary>
        /// Array of GPS locations for this employee/position.
        /// </summary>
        public List<EmployeePostLocationItemDto> EmployeePostLocations { get; set; } = new();
    }
}
