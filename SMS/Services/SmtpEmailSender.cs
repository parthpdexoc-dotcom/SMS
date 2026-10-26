using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Services.Interfaces;

namespace SMS.Services
{
    /// <summary>
    /// SMTP configuration for sending emails.
    /// </summary>
    public class SmtpSettings
    {
        public const string SectionName = "Smtp";
        public string? Host { get; set; }
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FromAddress { get; set; }
        public string? FromDisplayName { get; set; }
        /// <summary>Alternative key for FromAddress (appsettings: FromEmail).</summary>
        public string? FromEmail { get; set; }
        /// <summary>Alternative key for FromDisplayName (appsettings: FromName).</summary>
        public string? FromName { get; set; }
    }

    /// <summary>
    /// Sends email via SMTP using configuration from appsettings.
    /// </summary>
    public class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<SmtpSettings> options, ILogger<SmtpEmailSender> logger)
        {
            _settings = options?.Value ?? new SmtpSettings();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task SendAsync(
            IReadOnlyList<string> toAddresses,
            string subject,
            string body,
            bool isBodyHtml = true,
            EmailCalendarInviteDto? calendarInvite = null,
            IReadOnlyList<string>? ccAddresses = null,
            IReadOnlyList<string>? bccAddresses = null,
            IReadOnlyList<EmailAttachmentDto>? attachments = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.Host))
            {
                _logger.LogWarning(
                    "Skipping email '{Subject}': SMTP is not configured (Smtp:Host is missing).",
                    subject);
                return;
            }

            if (toAddresses == null || toAddresses.Count == 0)
            {
                _logger.LogWarning("Skipping email '{Subject}': no recipients provided.", subject);
                return;
            }

            var fromAddr = (_settings.FromAddress ?? _settings.FromEmail ?? "noreply@localhost").Trim();
            if (!EmailAddressHelper.IsValidEmail(fromAddr))
                fromAddr = "noreply@localhost";
            var fromName = (_settings.FromDisplayName ?? _settings.FromName ?? string.Empty).Trim();

            var deliverableTo = NormalizeDeliverableRecipients(toAddresses);
            var deliverableCc = NormalizeDeliverableRecipients(ccAddresses);
            var deliverableBcc = NormalizeDeliverableRecipients(bccAddresses);

            if (deliverableTo.Count == 0)
            {
                _logger.LogWarning(
                    "Skipping email '{Subject}' because all To addresses are missing, invalid, or non-deliverable placeholders.",
                    subject);
                return;
            }

            MimeMessage message;
            try
            {
                message = BuildMimeMessage(
                    fromAddr,
                    fromName,
                    deliverableTo,
                    deliverableCc,
                    deliverableBcc,
                    subject ?? string.Empty,
                    body,
                    isBodyHtml,
                    calendarInvite,
                    attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to build email '{Subject}' for {Count} recipient(s).", subject, deliverableTo.Count);
                return;
            }

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Timeout = 10000;

                var secureSocketOptions = _settings.Port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : (_settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                try
                {
                    await client.ConnectAsync(_settings.Host, _settings.Port, secureSocketOptions, cancellationToken);
                }
                catch (Exception connectEx) when (_settings.Port != 587)
                {
                    _logger.LogWarning(connectEx, "Failed to connect to SMTP {Host}:{Port}, attempting fallback to port 587 (StartTls)...", _settings.Host, _settings.Port);
                    if (!client.IsConnected)
                    {
                        try
                        {
                            await client.ConnectAsync(_settings.Host, 587, SecureSocketOptions.StartTls, cancellationToken);
                        }
                        catch
                        {
                            throw connectEx;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(_settings.UserName) && client.IsConnected)
                {
                    await client.AuthenticateAsync(
                        new NetworkCredential(_settings.UserName, _settings.Password),
                        cancellationToken);
                }

                if (client.IsConnected)
                {
                    await client.SendAsync(message, cancellationToken);
                    await client.DisconnectAsync(true, cancellationToken);
                    _logger.LogInformation(
                        "Email sent to {ToCount} To, {CcCount} Cc, {BccCount} Bcc recipient(s), subject: {Subject}",
                        deliverableTo.Count,
                        deliverableCc.Count,
                        deliverableBcc.Count,
                        subject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to send email to {ToCount} To, {CcCount} Cc, {BccCount} Bcc recipient(s), subject: {Subject}",
                    deliverableTo.Count,
                    deliverableCc.Count,
                    deliverableBcc.Count,
                    subject);
            }
        }

        private static List<string> NormalizeDeliverableRecipients(IReadOnlyList<string>? addresses) =>
            addresses?
                .Where(e => !string.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())
                .Where(IsDeliverableRecipientAddress)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

        private static bool TryParseMailboxAddress(string address, out MailboxAddress mailbox)
        {
            mailbox = default!;
            return !string.IsNullOrWhiteSpace(address) && MailboxAddress.TryParse(address.Trim(), out mailbox);
        }

        private static bool IsDeliverableRecipientAddress(string address)
        {
            if (!EmailAddressHelper.IsValidEmail(address))
                return false;

            var atIndex = address.IndexOf('@');
            if (atIndex <= 0)
                return false;

            var localPart = address[..atIndex].Trim();
            return !localPart.Equals("noreply", StringComparison.OrdinalIgnoreCase)
                && !localPart.Equals("no-reply", StringComparison.OrdinalIgnoreCase)
                && !localPart.Equals("do-not-reply", StringComparison.OrdinalIgnoreCase)
                && !localPart.Equals("donotreply", StringComparison.OrdinalIgnoreCase);
        }

        private static MimeMessage BuildMimeMessage(
            string fromAddress,
            string fromName,
            IReadOnlyList<string> toAddresses,
            IReadOnlyList<string> ccAddresses,
            IReadOnlyList<string> bccAddresses,
            string subject,
            string? body,
            bool isBodyHtml,
            EmailCalendarInviteDto? calendarInvite,
            IReadOnlyList<EmailAttachmentDto>? attachments)
        {
            var message = new MimeMessage();
            if (!TryParseMailboxAddress(fromAddress, out var fromMailbox))
                fromMailbox = new MailboxAddress(fromName, "noreply@localhost");
            else if (!string.IsNullOrWhiteSpace(fromName))
                fromMailbox = new MailboxAddress(fromName, fromMailbox.Address);
            message.From.Add(fromMailbox);
            message.Subject = subject;

            AddMailboxAddresses(message.To, toAddresses);
            AddMailboxAddresses(message.Cc, ccAddresses);
            AddMailboxAddresses(message.Bcc, bccAddresses);

            if (message.To.Count == 0)
                throw new InvalidOperationException("No valid recipient addresses could be parsed for the email message.");

            var content = body ?? string.Empty;
            if (calendarInvite != null)
            {
                content = isBodyHtml
                    ? AppendInviteHtmlSection(content, calendarInvite)
                    : AppendInviteTextSection(content, calendarInvite);
            }

            var plainTextBody = ConvertHtmlToPlainText(content);
            var hasAttachments = attachments != null && attachments.Any(a => a.Content.Length > 0);

            if (calendarInvite == null)
            {
                message.Body = BuildBodyWithoutCalendar(content, plainTextBody, isBodyHtml, attachments);
                return message;
            }

            var alternative = new Multipart("alternative")
            {
                new TextPart(TextFormat.Plain) { Text = isBodyHtml ? plainTextBody : content }
            };

            if (isBodyHtml)
            {
                alternative.Add(new TextPart(TextFormat.Html) { Text = content });
            }

            var calendarPart = new TextPart("calendar")
            {
                Text = BuildIcsContent(calendarInvite, fromAddress, toAddresses),
                ContentTransferEncoding = ContentEncoding.SevenBit
            };
            calendarPart.ContentType.Parameters.Add("method", "REQUEST");
            calendarPart.ContentType.Parameters.Add("component", "VEVENT");
            calendarPart.ContentType.Charset = Encoding.UTF8.WebName;
            alternative.Add(calendarPart);

            message.Body = hasAttachments
                ? WrapWithAttachments(alternative, attachments!)
                : alternative;
            message.Headers.Replace(HeaderId.ContentClass, "urn:content-classes:calendarmessage");
            return message;
        }

        private static void AddMailboxAddresses(InternetAddressList target, IReadOnlyList<string> addresses)
        {
            foreach (var address in addresses)
            {
                if (TryParseMailboxAddress(address, out var mailbox))
                    target.Add(mailbox);
            }
        }

        private static MimeEntity BuildBodyWithoutCalendar(
            string content,
            string plainTextBody,
            bool isBodyHtml,
            IReadOnlyList<EmailAttachmentDto>? attachments)
        {
            if (attachments == null || !attachments.Any(a => a.Content.Length > 0))
            {
                return isBodyHtml
                    ? new BodyBuilder { TextBody = plainTextBody, HtmlBody = content }.ToMessageBody()
                    : new TextPart(TextFormat.Plain) { Text = content };
            }

            var builder = new BodyBuilder();
            if (isBodyHtml)
            {
                builder.HtmlBody = content;
                builder.TextBody = plainTextBody;
            }
            else
            {
                builder.TextBody = content;
            }

            AddAttachments(builder, attachments);
            return builder.ToMessageBody();
        }

        private static Multipart WrapWithAttachments(MimeEntity body, IReadOnlyList<EmailAttachmentDto> attachments)
        {
            var mixed = new Multipart("mixed") { body };
            foreach (var attachment in attachments.Where(a => a.Content.Length > 0))
            {
                mixed.Add(CreateAttachmentPart(attachment));
            }

            return mixed;
        }

        private static void AddAttachments(BodyBuilder builder, IReadOnlyList<EmailAttachmentDto> attachments)
        {
            foreach (var attachment in attachments.Where(a => a.Content.Length > 0))
            {
                var fileName = string.IsNullOrWhiteSpace(attachment.FileName) ? "attachment" : attachment.FileName;
                var contentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                    ? MediaTypeNames.Application.Octet
                    : attachment.ContentType;
                builder.Attachments.Add(fileName, attachment.Content, MimeKit.ContentType.Parse(contentType));
            }
        }

        private static MimePart CreateAttachmentPart(EmailAttachmentDto attachment)
        {
            var fileName = string.IsNullOrWhiteSpace(attachment.FileName) ? "attachment" : attachment.FileName;
            var contentType = string.IsNullOrWhiteSpace(attachment.ContentType)
                ? MediaTypeNames.Application.Octet
                : attachment.ContentType;

            var part = new MimePart(MimeKit.ContentType.Parse(contentType))
            {
                Content = new MimeContent(new MemoryStream(attachment.Content)),
                ContentDisposition = new MimeKit.ContentDisposition(MimeKit.ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName
            };
            return part;
        }

        private static string ConvertHtmlToPlainText(string html) =>
            HtmlTextHelper.ConvertHtmlToPlainText(html);

        private static string AppendInviteHtmlSection(string htmlBody, EmailCalendarInviteDto invite)
        {
            var (googleUrl, outlookUrl) = BuildCalendarLinks(invite);
            var startLocal = invite.StartUtc.ToLocalTime();
            var endLocal = invite.EndUtc.ToLocalTime();

            var section = $@"
<hr />
<div style=""font-family:Segoe UI,Arial,sans-serif; border:1px solid #dfe1e6; border-radius:8px; padding:12px; margin-top:12px;"">
  <div style=""font-size:16px; font-weight:600; margin-bottom:8px;"">Meeting Invitation</div>
  <div><strong>Title:</strong> {WebUtility.HtmlEncode(invite.Summary)}</div>
  <div><strong>When:</strong> {WebUtility.HtmlEncode(startLocal.ToString("MMM dd, yyyy h:mm tt", CultureInfo.InvariantCulture))} - {WebUtility.HtmlEncode(endLocal.ToString("h:mm tt", CultureInfo.InvariantCulture))}</div>
  <div><strong>Location:</strong> {WebUtility.HtmlEncode(invite.Location ?? string.Empty)}</div>
  <div style=""margin-top:10px;"">
    <a href=""{WebUtility.HtmlEncode(googleUrl)}"" style=""margin-right:12px;"">Add to Google Calendar</a>
    <a href=""{WebUtility.HtmlEncode(outlookUrl)}"">Add to Outlook</a>
  </div>
</div>";

            return (htmlBody ?? string.Empty) + section;
        }

        private static string AppendInviteTextSection(string textBody, EmailCalendarInviteDto invite)
        {
            var (googleUrl, outlookUrl) = BuildCalendarLinks(invite);
            var startLocal = invite.StartUtc.ToLocalTime();
            var endLocal = invite.EndUtc.ToLocalTime();

            return (textBody ?? string.Empty) +
                $"\n\nMeeting Invitation\n" +
                $"Title: {invite.Summary}\n" +
                $"When: {startLocal.ToString("MMM dd, yyyy h:mm tt", CultureInfo.InvariantCulture)} - {endLocal.ToString("h:mm tt", CultureInfo.InvariantCulture)}\n" +
                $"Location: {invite.Location}\n" +
                $"Add to Google Calendar: {googleUrl}\n" +
                $"Add to Outlook: {outlookUrl}\n";
        }

        private static (string GoogleUrl, string OutlookUrl) BuildCalendarLinks(EmailCalendarInviteDto invite)
        {
            var startUtc = invite.StartUtc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
            var endUtc = invite.EndUtc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");

            var googleUrl =
                $"https://calendar.google.com/calendar/render?action=TEMPLATE" +
                $"&text={Uri.EscapeDataString(invite.Summary ?? string.Empty)}" +
                $"&dates={startUtc}/{endUtc}" +
                $"&details={Uri.EscapeDataString(invite.Description ?? string.Empty)}" +
                $"&location={Uri.EscapeDataString(invite.Location ?? string.Empty)}";

            var outlookUrl =
                $"https://outlook.live.com/calendar/0/deeplink/compose?path=%2Fcalendar%2Faction%2Fcompose" +
                $"&rru=addevent" +
                $"&subject={Uri.EscapeDataString(invite.Summary ?? string.Empty)}" +
                $"&startdt={Uri.EscapeDataString(invite.StartUtc.ToUniversalTime().ToString("o"))}" +
                $"&enddt={Uri.EscapeDataString(invite.EndUtc.ToUniversalTime().ToString("o"))}" +
                $"&body={Uri.EscapeDataString(invite.Description ?? string.Empty)}" +
                $"&location={Uri.EscapeDataString(invite.Location ?? string.Empty)}";

            return (googleUrl, outlookUrl);
        }

        private static string BuildIcsContent(
            EmailCalendarInviteDto invite,
            string fromAddress,
            IReadOnlyList<string> toAddresses)
        {
            static string Escape(string? value) =>
                (value ?? string.Empty)
                    .Replace("\\", "\\\\", StringComparison.Ordinal)
                    .Replace(";", "\\;", StringComparison.Ordinal)
                    .Replace(",", "\\,", StringComparison.Ordinal)
                    .Replace("\r\n", "\\n", StringComparison.Ordinal)
                    .Replace("\n", "\\n", StringComparison.Ordinal);

            static string EscapeParam(string? value) =>
                (value ?? string.Empty)
                    .Replace("\\", "\\\\", StringComparison.Ordinal)
                    .Replace("\"", "\\\"", StringComparison.Ordinal)
                    .Replace(";", "\\;", StringComparison.Ordinal)
                    .Replace(",", "\\,", StringComparison.Ordinal)
                    .Replace("\r\n", " ", StringComparison.Ordinal)
                    .Replace("\n", " ", StringComparison.Ordinal);

            static string FoldLine(string line)
            {
                // RFC 5545 line folding: max 75 octets; folded lines start with one space.
                const int maxCharsPerLine = 73;
                if (line.Length <= maxCharsPerLine)
                    return line;

                var folded = new StringBuilder();
                var index = 0;
                while (index < line.Length)
                {
                    var len = Math.Min(maxCharsPerLine, line.Length - index);
                    if (index > 0)
                        folded.Append("\r\n ");
                    folded.Append(line.AsSpan(index, len));
                    index += len;
                }

                return folded.ToString();
            }

            var dtStamp = DateTime.UtcNow.ToString("yyyyMMdd'T'HHmmss'Z'");
            var dtStart = invite.StartUtc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
            var dtEnd = invite.EndUtc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'");
            var created = dtStamp;
            var lastModified = dtStamp;
            var organizerCn = Escape("SMS System");
            var attendeeLines = toAddresses
                .Select(email =>
                    $"ATTENDEE;CUTYPE=INDIVIDUAL;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"{EscapeParam(email)}\":MAILTO:{Escape(email)}")
                .ToArray();
            var lines = new[]
            {
                "BEGIN:VCALENDAR",
                "PRODID:-//SMS//Post Assignment//EN",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:REQUEST",
                "X-WR-CALNAME:SMS Assignments",
                "BEGIN:VEVENT",
                $"UID:{Escape(invite.Uid)}",
                $"DTSTAMP:{dtStamp}",
                $"CREATED:{created}",
                $"LAST-MODIFIED:{lastModified}",
                $"DTSTART:{dtStart}",
                $"DTEND:{dtEnd}",
                $"SUMMARY:{Escape(invite.Summary)}",
                $"DESCRIPTION:{Escape(invite.Description)}",
                $"LOCATION:{Escape(invite.Location)}",
                "CLASS:PUBLIC",
                "PRIORITY:5",
                $"ORGANIZER;CN=\"{organizerCn}\":MAILTO:{Escape(fromAddress)}",
                "X-MICROSOFT-CDO-IMPORTANCE:1",
                "X-MICROSOFT-CDO-BUSYSTATUS:BUSY",
                "X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY",
                "SEQUENCE:0",
                "STATUS:CONFIRMED",
                "TRANSP:OPAQUE",
            }.Concat(attendeeLines).Concat(new[]
            {
                "END:VEVENT",
                "END:VCALENDAR",
                string.Empty
            });

            return string.Join("\r\n", lines.Select(FoldLine));
        }
    }
}
