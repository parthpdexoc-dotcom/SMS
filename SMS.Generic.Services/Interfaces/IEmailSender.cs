using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Abstraction for sending email messages.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email to one or more recipients. From address and display name are taken from appsettings (Smtp:FromEmail/FromAddress, Smtp:FromName/FromDisplayName).
        /// </summary>
        /// <param name="toAddresses">Recipient email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email body (HTML or plain text).</param>
        /// <param name="isBodyHtml">True if body is HTML; otherwise false.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Best-effort: invalid addresses and SMTP failures are logged and do not throw.</remarks>
        Task SendAsync(
            IReadOnlyList<string> toAddresses,
            string subject,
            string body,
            bool isBodyHtml = true,
            EmailCalendarInviteDto? calendarInvite = null,
            IReadOnlyList<string>? ccAddresses = null,
            IReadOnlyList<string>? bccAddresses = null,
            IReadOnlyList<EmailAttachmentDto>? attachments = null,
            CancellationToken cancellationToken = default);
    }
}
