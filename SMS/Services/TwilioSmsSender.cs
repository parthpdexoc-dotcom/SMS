using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Services.Interfaces;

namespace SMS.Services
{
    public class TwilioSettings
    {
        public const string SectionName = "Twilio";

        /// <summary>Twilio REST base URL. The Messages endpoint is built from this plus the Account SID.</summary>
        public string BaseUrl { get; set; } = "https://api.twilio.com/2010-04-01";

        public string? AccountSid { get; set; }

        /// <summary>API Key SID (starts with SK). Used as the Basic-auth username.</summary>
        public string? ApiKey { get; set; }

        /// <summary>API Key secret. Used as the Basic-auth password. Shown only once when the key is created.</summary>
        public string? ApiSecret { get; set; }

        /// <summary>Account auth token. Used to validate inbound STOP/HELP signatures, not to send.</summary>
        public string? AuthToken { get; set; }

        /// <summary>Dedicated sender in E.164, for example +19803693085.</summary>
        public string? FromPhoneNumber { get; set; }

        /// <summary>Twilio phone-number SID (starts with PN) for the dedicated sender. Not sent on the message.</summary>
        public string? PhoneNumberSid { get; set; }

        /// <summary>
        /// Public HTTPS URL Twilio calls for inbound STOP/HELP, with no trailing slash.
        /// Used to validate the Twilio signature when the API sits behind a proxy.
        /// Example: https://api.example.com/api/twilio/sms/inbound
        /// </summary>
        public string? WebhookUrl { get; set; }

        /// <summary>Customer support phone shown in inbound HELP replies.</summary>
        public string CustomerSupportPhone { get; set; } = "888-430-4007";

        /// <summary>
        /// Inbound HELP/INFO reply template. Replace <c>{SupportPhone}</c> with <see cref="CustomerSupportPhone"/>.
        /// </summary>
        public string? InboundHelpMessageTemplate { get; set; }

        /// <summary>Inbound STOP (and related keywords) reply template.</summary>
        public string? InboundStopMessageTemplate { get; set; }

        /// <summary>Resolves the TwiML body returned when a user texts HELP or INFO.</summary>
        public string ResolveInboundHelpMessage()
        {
            var supportPhone = string.IsNullOrWhiteSpace(CustomerSupportPhone)
                ? "888-430-4007"
                : CustomerSupportPhone.Trim();

            var template = string.IsNullOrWhiteSpace(InboundHelpMessageTemplate)
                ? "If you have questions about the Job please contact your Agency Administrator. If you have questions about the system contact customer support at {SupportPhone}"
                : InboundHelpMessageTemplate.Trim();

            return template.Replace("{SupportPhone}", supportPhone, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Resolves the TwiML body returned when a user texts STOP or other opt-out keywords.</summary>
        public string ResolveInboundStopMessage()
        {
            if (!string.IsNullOrWhiteSpace(InboundStopMessageTemplate))
                return InboundStopMessageTemplate.Trim();

            return "You will no longer receive schedule texts from CheckTheSite.com.";
        }
    }

    public class TwilioSmsSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly TwilioSettings _settings;
        private readonly ILogger<TwilioSmsSender> _logger;

        public TwilioSmsSender(
            HttpClient httpClient,
            IOptions<TwilioSettings> settings,
            ILogger<TwilioSmsSender> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(string toPhoneNumber, string body, CancellationToken cancellationToken = default)
        {
            var username = string.IsNullOrWhiteSpace(_settings.ApiKey) ? _settings.AccountSid : _settings.ApiKey;
            var password = string.IsNullOrWhiteSpace(_settings.ApiSecret) ? _settings.AuthToken : _settings.ApiSecret;
            if (string.IsNullOrWhiteSpace(_settings.AccountSid)
                || string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(password)
                || string.IsNullOrWhiteSpace(_settings.FromPhoneNumber))
            {
                _logger.LogWarning("SMS was not sent because Twilio AccountSid, AuthToken, or FromPhoneNumber is not configured.");
                return;
            }

            var to = ToE164(toPhoneNumber);
            var from = ToE164(_settings.FromPhoneNumber) ?? ToUsE164(_settings.FromPhoneNumber);
            if (to == null || from == null)
            {
                _logger.LogWarning("SMS was not sent because the destination or Twilio From number could not be formatted.");
                return;
            }

            if (string.IsNullOrWhiteSpace(body))
                return;

            var baseUrl = string.IsNullOrWhiteSpace(_settings.BaseUrl)
                ? "https://api.twilio.com/2010-04-01"
                : _settings.BaseUrl.TrimEnd('/');
            var url = $"{baseUrl}/Accounts/{_settings.AccountSid.Trim()}/Messages.json";
            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username.Trim()}:{password.Trim()}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["To"] = to,
                ["From"] = from,
                ["Body"] = body.Trim()
            });

            try
            {
                using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogError("Twilio SMS to {To} failed with status {Status}. {Error}", to, (int)response.StatusCode, error);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Twilio SMS to {To} failed.", to);
            }
        }

        /// <summary>
        /// Accepts a number that already includes its country code. Does not add or remove one.
        /// </summary>
        public static string? ToE164(string? phone)
        {
            if (!PhoneNumberHelper.HasValidE164(phone) || string.IsNullOrWhiteSpace(phone))
                return null;

            return PhoneNumberHelper.NormalizeE164(phone);
        }

        /// <summary>Old sender values are stored as 10 US digits. Those are sent as +1.</summary>
        private static string? ToUsE164(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone) || phone.Trim().StartsWith('+'))
                return null;

            var digits = new string(phone.Where(char.IsDigit).ToArray());
            return digits.Length == 10 ? "+1" + digits : null;
        }

        public static bool IsValidSignature(
            string authToken,
            string url,
            IEnumerable<KeyValuePair<string, string>> formFields,
            string signature)
        {
            if (string.IsNullOrWhiteSpace(authToken) || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(signature))
                return false;

            var payload = new StringBuilder(url);
            foreach (var field in formFields.OrderBy(f => f.Key, StringComparer.Ordinal))
                payload.Append(field.Key).Append(field.Value);

            using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(authToken));
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload.ToString())));
            var expected = Encoding.UTF8.GetBytes(hash);
            var actual = Encoding.UTF8.GetBytes(signature);
            return expected.Length == actual.Length && CryptographicOperations.FixedTimeEquals(expected, actual);
        }
    }
}
