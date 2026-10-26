namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// DTO representing an employee for notification.
    /// </summary>
    public class NotificationEmployeeDto
    {
        public Guid EmployeeId { get; set; }

        public string? EmployeeName { get; set; }

        public string? EmployeeNumber { get; set; }

        public string? PrimaryEmail { get; set; }

        public string? OffDutyCallBackEmail { get; set; }

        public Guid? PostPositionId { get; set; }

        public DateOnly? DateOfWork { get; set; }

        public bool IsPlaceholder { get; set; }
    }
}
