using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmailCommunication operations.
    /// </summary>
    public class EmailCommunicationDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        public string? EmailBody { get; set; }

        /// <summary>
        /// Gets or sets whether the email has been sent.
        /// </summary>
        public bool? Sent { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the email communication.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the requester ID associated with this email.
        /// </summary>
        public int? RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }
    }
}

