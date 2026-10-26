using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmailProcess operations.
    /// </summary>
    public class EmailProcessDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the email process.
        /// </summary>
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
        [MaxLength(50, ErrorMessage = "EmailType cannot exceed 50 characters")]
        public string? EmailType { get; set; }

        /// <summary>
        /// Gets or sets the email from address.
        /// </summary>
        [MaxLength(100, ErrorMessage = "EmailFrom cannot exceed 100 characters")]
        public string? EmailFrom { get; set; }

        /// <summary>
        /// Gets or sets the email to address.
        /// </summary>
        [MaxLength(100, ErrorMessage = "EmailTo cannot exceed 100 characters")]
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
        [MaxLength(300, ErrorMessage = "EmailSubject cannot exceed 300 characters")]
        public string? EmailSubject { get; set; }

        /// <summary>
        /// Gets or sets the email body.
        /// </summary>
        public string? EmailBody { get; set; }

        /// <summary>
        /// Gets or sets the email message.
        /// </summary>
        [MaxLength(250, ErrorMessage = "EmailMsg cannot exceed 250 characters")]
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

