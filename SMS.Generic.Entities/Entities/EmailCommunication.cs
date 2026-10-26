using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an email communication entity in the system.
    /// </summary>
    [Table("EmailCommunication", Schema = "dbo")]
    public class EmailCommunication
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public string? Email { get; set; }
    }
}

