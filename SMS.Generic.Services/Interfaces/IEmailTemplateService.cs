using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for email template service operations.
    /// </summary>
    public interface IEmailTemplateService : IGenericService<EmailTemplateDto, EmailTemplate>
    {
        /// <summary>
        /// Sends email(s) using the template identified by the request's EmailType, with optional parameter replacement.
        /// </summary>
        /// <param name="request">The send mail request (EmailType, Email, optional FromEmail, optional TemplateParameters).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Best-effort: failures are logged and do not throw to callers.</remarks>
        Task SendMailAsync(SendMailRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets template keywords used for a specific email type.
        /// </summary>
        /// <param name="emailType">Email type to fetch keywords for.</param>
        /// <returns>Distinct keyword list used by the email type.</returns>
        IReadOnlyList<string> GetKeywordsByEmailType(EmailType emailType);

        /// <summary>
        /// Gets all email types that have keyword mapping metadata, with numeric value and display label.
        /// </summary>
        /// <returns>Email type options in enum value order.</returns>
        IReadOnlyList<EmailTypeOptionDto> GetEmailTypes();

        /// <summary>
        /// Notifies department admin employees for the employee's department that an outbound email could not be delivered.
        /// </summary>
        Task SendEmployeeEmailUnavailableNotificationAsync(
            EmployeeDto employee,
            EmailType failedEmailType,
            CancellationToken cancellationToken = default);
    }
}
