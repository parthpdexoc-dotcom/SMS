using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    [Table("NotificationHistory", Schema = "dbo")]
    public class NotificationHistory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NotificationHistoryID { get; set; }

        public Guid DepartmentID { get; set; }

        public Guid? RequestID { get; set; }

        public Guid? RequesterID { get; set; }

        public Guid? PostID { get; set; }

        public Guid? PostPositionID { get; set; }

        public NotificationContextType ContextType { get; set; }

        [MaxLength(500)]
        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public NotificationStatus Status { get; set; }

        [MaxLength(1000)]
        public string? FailureMessage { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        public ICollection<NotificationHistoryRecipient> Recipients { get; set; } = new List<NotificationHistoryRecipient>();

        public ICollection<NotificationHistoryAttachment> Attachments { get; set; } = new List<NotificationHistoryAttachment>();
    }
}
