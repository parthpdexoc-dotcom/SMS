using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an email template entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(Title), OnlyAttributedProperties = true)]
    [Table("EmailTemplate", Schema = "dbo")]
    public class EmailTemplate : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmailTemplateID { get; set; }

        /// <summary>
        /// Title of the email template.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Title")]
        public string? Title { get; set; }

        /// <summary>
        /// Subject line of the email.
        /// </summary>
        [MaxLength(500)]
        [AuditedProperty("Subject")]
        public string? Subject { get; set; }

        /// <summary>
        /// Body content of the email template.
        /// </summary>
        [AuditedProperty("Content")]
        public string? Content { get; set; }

        /// <summary>
        /// SMS body content of the template.
        /// </summary>
        [AuditedProperty("SMS Content")]
        public string? SmsContent { get; set; }

        /// <summary>
        /// Type of the email template.
        /// </summary>
        [AuditedProperty("Email type")]
        public EmailType? EmailType { get; set; }
    }
}
