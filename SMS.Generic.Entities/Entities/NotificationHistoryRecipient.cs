using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    [Table("NotificationHistoryRecipient", Schema = "dbo")]
    public class NotificationHistoryRecipient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NotificationHistoryRecipientID { get; set; }

        public Guid NotificationHistoryID { get; set; }

        [ForeignKey(nameof(NotificationHistoryID))]
        public NotificationHistory? NotificationHistory { get; set; }

        public NotificationRecipientType RecipientType { get; set; }

        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public Guid? EmployeeID { get; set; }

        [MaxLength(255)]
        public string? EmployeeName { get; set; }
    }
}
