using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request DTO for sending notification emails.
    /// </summary>
    public class SendNotificationRequestDto
    {
        /// <summary>
        /// Email subject.
        /// </summary>
        [Required]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Email body (HTML or plain text).
        /// </summary>
        [Required]
        public string Body { get; set; } = string.Empty;

        /// <summary>
        /// Primary recipient email addresses.
        /// </summary>
        [Required]
        public List<string> To { get; set; } = new();

        /// <summary>
        /// CC recipient email addresses.
        /// </summary>
        public List<string>? Cc { get; set; }

        /// <summary>
        /// BCC recipient email addresses.
        /// </summary>
        public List<string>? Bcc { get; set; }

        /// <summary>
        /// Request ID (required when <see cref="AttachmentFileIds"/> is used to resolve uploaded files).
        /// </summary>
        public Guid? RequestId { get; set; }

        /// <summary>
        /// Requester ID (required when <see cref="AttachmentFileIds"/> is used to resolve uploaded files).
        /// </summary>
        public Guid? RequesterId { get; set; }

        /// <summary>
        /// IDs of files previously uploaded via the upload API; resolved from disk and attached to the email.
        /// </summary>
        public List<string>? AttachmentFileIds { get; set; }

        public Guid? PostId { get; set; }

        public Guid? PostPositionId { get; set; }

        public NotificationContextType? ContextType { get; set; }
    }
}
