using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request DTO for sending assign-post email and push notification.
    /// </summary>
    public class SendAssignPostEmailRequestDto
    {
        [Required]
        public Guid EmployeeId { get; set; }

        public string? Email { get; set; }

        [Required]
        public string PostTitle { get; set; } = string.Empty;

        public DateOnly? DateOfWork { get; set; }
        public TimeOnly? StartTime { get; set; }
        public int? PostLength { get; set; }
        public string? PostLocation { get; set; }
        public string? AssignedEmployeeName { get; set; }
        public string? AssignedEmployeeType { get; set; }
        public string? AssignedEmployeeRank { get; set; }
        public string? PushNotificationTitle { get; set; }
        public string? PushNotificationBody { get; set; }
    }
}
