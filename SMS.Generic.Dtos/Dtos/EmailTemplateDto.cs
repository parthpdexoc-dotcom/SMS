using System.ComponentModel.DataAnnotations;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmailTemplate operations.
    /// </summary>
    public class EmailTemplateDto : BaseDto 
    {

        /// <summary>
        /// Primary Key - Email Template ID (GUID)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("emailTemplateId")]
        [System.Text.Json.Serialization.JsonPropertyName("emailTemplateId")]
        public Guid? EmailTemplateID { get; set; }

        /// <summary>
        /// Gets or sets the title of the email template.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the subject line of the email.
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the body content of the email template.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Gets or sets the SMS body content of the template.
        /// </summary>
        public string? SmsContent { get; set; }

        /// <summary>
        /// Gets or sets the type of the email template.
        /// </summary>
        public EmailType? EmailType { get; set; }
    }
}
