using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Post position slice for scheduled / ready-to-close modal.</summary>
    public class ViewDetailsPostPositionInfo
    {
        public Guid PostPositionId { get; set; }
        public Guid PostId { get; set; }
        public DateOnly? DateOfWork { get; set; }
        public TimeOnly? PostStartTime { get; set; }
        public TimeOnly PostEndTime { get; set; }
        public int LengthOfWork { get; set; }
        public PostPositionStatus PostPositionStatus { get; set; }
        public string? CallSign { get; set; }
        public string? EmployeeFirstName { get; set; }
        public string? EmployeeLastName { get; set; }
        public decimal PostPositionPayRate { get; set; }
        public bool IsPremiumPaidEvent { get; set; }
    }
}
