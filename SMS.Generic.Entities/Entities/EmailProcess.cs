using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an email process entity in the system.
    /// </summary>
    [Table("EmailProcess", Schema = "dbo")]
    public class EmailProcess
    {
        /// <summary>
        /// Gets or sets the unique identifier for the email process.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailProcessID { get; set; }

        /// <summary>
        /// Gets or sets the email department identifier.
        /// </summary>
        public int? EmailDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the email employee identifier.
        /// </summary>
        public int? EmailEmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the email type.
        /// </summary>
        [MaxLength(50)]
        public string? EmailType { get; set; }

        /// <summary>
        /// Gets or sets the email from address.
        /// </summary>
        [MaxLength(100)]
        public string? EmailFrom { get; set; }

        /// <summary>
        /// Gets or sets the email to address.
        /// </summary>
        [MaxLength(100)]
        public string? EmailTo { get; set; }

        /// <summary>
        /// Gets or sets the email CC addresses.
        /// </summary>
        public string? EmailCc { get; set; }

        /// <summary>
        /// Gets or sets the email BCC addresses.
        /// </summary>
        public string? EmailBcc { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        [MaxLength(300)]
        public string? EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets the email body.
        /// </summary>
        public string? EmailBody { get; set; }

        /// <summary>
        /// Gets or sets the email message.
        /// </summary>
        [MaxLength(250)]
        public string? EmailMsg { get; set; }

        /// <summary>
        /// Gets or sets the email timestamp.
        /// </summary>
        public DateTime? EmailTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the email sending timestamp.
        /// </summary>
        public DateTime? EmailSendingTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets whether the email is in progress.
        /// </summary>
        public bool? EmailInProgress { get; set; }

        /// <summary>
        /// Gets or sets the email status.
        /// </summary>
        public bool? EmailStatus { get; set; }

        /// <summary>
        /// Gets or sets the email post day.
        /// </summary>
        public DateTime? EmailPostDay { get; set; }

        /// <summary>
        /// Gets or sets the email post identifier.
        /// </summary>
        public int? EmailPostID { get; set; }
    }
}

