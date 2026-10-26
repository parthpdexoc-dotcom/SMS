using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    [Table("NotificationHistoryAttachment", Schema = "dbo")]
    public class NotificationHistoryAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid NotificationHistoryAttachmentID { get; set; }

        public Guid NotificationHistoryID { get; set; }

        [ForeignKey(nameof(NotificationHistoryID))]
        public NotificationHistory? NotificationHistory { get; set; }

        public Guid? FileUploadedId { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        public long? FileSizeBytes { get; set; }

        [MaxLength(100)]
        public string? ContentType { get; set; }
    }
}
