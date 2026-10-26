using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    public class NotificationHistoryRecipientDto
    {
        public Guid? NotificationHistoryRecipientID { get; set; }

        public Guid? NotificationHistoryID { get; set; }

        public NotificationRecipientType? RecipientType { get; set; }

        public string Email { get; set; } = string.Empty;

        public Guid? EmployeeID { get; set; }

        public string? EmployeeName { get; set; }
    }
}
