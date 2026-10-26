using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request DTO for sending email using a template identified by EmailType.
    /// </summary>
    public class SendMailRequestDto
    {
        /// <summary>
        /// The type of email template to use (used to find the template in the database).
        /// </summary>
        public EmailType EmailType { get; set; }

        /// <summary>
        /// Recipient email addresses.
        /// </summary>
        public List<string> Emails { get; set; } = new List<string>();

        /// <summary>
        /// Optional sender (From) email address. When set, the email is sent with this as the From address.
        /// </summary>
        public string? FromEmail { get; set; }

        /// <summary>
        /// Optional template parameters. Keys match placeholders in the template (e.g. "FullName", "DepartmentName").
        /// Placeholders in template are in the form [FullName], [DepartmentName].
        /// </summary>
        public Dictionary<string, string>? TemplateParameters { get; set; }

        /// <summary>
        /// Optional calendar invite that will be attached as an ICS file.
        /// </summary>
        public EmailCalendarInviteDto? CalendarInvite { get; set; }

        /// <summary>
        /// Parameterless constructor for serialization and model binding.
        /// </summary>
        public SendMailRequestDto()
        {
        }

        /// <summary>
        /// Constructor for creating a send mail request with all fields.
        /// </summary>
        /// <param name="emailType">The email template type.</param>
        /// <param name="emails">Recipient email addresses.</param>
        /// <param name="fromEmail">Optional From email address.</param>
        /// <param name="templateParameters">Optional template parameters.</param>
        public SendMailRequestDto(EmailType emailType, IEnumerable<string> emails, string? fromEmail, Dictionary<string, string>? templateParameters)
        {
            EmailType = emailType;
            Emails = emails?
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();
            FromEmail = string.IsNullOrWhiteSpace(fromEmail) ? null : fromEmail.Trim();
            TemplateParameters = templateParameters;
        }

        /// <summary>
        /// Convenience constructor for a single recipient email address.
        /// </summary>
        public SendMailRequestDto(EmailType emailType, string email, string? fromEmail, Dictionary<string, string>? templateParameters)
            : this(emailType, new[] { email }, fromEmail, templateParameters)
        {
        }
    }
}
