using System.Linq.Expressions;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Services.Interfaces;
using SMS.Services;

namespace SMS.Controllers
{
    /// <summary>
    /// Receives inbound Twilio SMS for STOP and HELP. Twilio calls this without a user login.
    /// </summary>
    [ApiController]
    [Route("api/twilio/sms")]
    public class TwilioSmsController : ControllerBase
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "STOP", "STOPALL", "UNSUBSCRIBE", "CANCEL", "END", "QUIT"
        };

        private static readonly HashSet<string> HelpWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "HELP", "INFO"
        };

        private readonly IEmployeeService _employeeService;
        private readonly TwilioSettings _settings;
        private readonly ILogger<TwilioSmsController> _logger;

        public TwilioSmsController(
            IEmployeeService employeeService,
            IOptions<TwilioSettings> settings,
            ILogger<TwilioSmsController> logger)
        {
            _employeeService = employeeService;
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("inbound")]
        public async Task<IActionResult> Inbound(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.AuthToken))
                return StatusCode(StatusCodes.Status503ServiceUnavailable);

            var form = await Request.ReadFormAsync(cancellationToken).ConfigureAwait(false);
            var signature = Request.Headers["X-Twilio-Signature"].ToString();
            var fields = form.Select(f => new KeyValuePair<string, string>(f.Key, f.Value.ToString()));
            if (!TwilioSmsSender.IsValidSignature(_settings.AuthToken, RequestUrl(), fields, signature))
            {
                _logger.LogWarning("Rejected inbound Twilio SMS because the signature did not match.");
                return Unauthorized();
            }

            var from = form["From"].ToString();
            var command = form["Body"].ToString().Trim();
            var phone = TwilioSmsSender.ToE164(from);

            if (phone != null && StopWords.Contains(command))
            {
                Expression<Func<Employee, bool>> matchesPhone = e =>
                    e.HasSmsConsent && e.SmsTelephoneNumber == phone;

                var updated = await _employeeService.UpdateAsync(
                    matchesPhone,
                    employee => employee.HasSmsConsent = false).ConfigureAwait(false);

                _logger.LogInformation("Twilio STOP from {Phone} cleared SMS consent on {Count} employee row(s).", phone, updated);
                return TwilioMessage(_settings.ResolveInboundStopMessage());
            }

            if (HelpWords.Contains(command))
            {
                return TwilioMessage(_settings.ResolveInboundHelpMessage());
            }

            return TwilioMessage(string.Empty);
        }

        private string RequestUrl()
        {
            if (!string.IsNullOrWhiteSpace(_settings.WebhookUrl))
                return _settings.WebhookUrl.TrimEnd('/') + Request.QueryString;

            return $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}{Request.QueryString}";
        }

        private ContentResult TwilioMessage(string body)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response>");
            if (!string.IsNullOrWhiteSpace(body))
            {
                xml.Append("<Message>");
                xml.Append(System.Security.SecurityElement.Escape(body));
                xml.Append("</Message>");
            }
            xml.Append("</Response>");
            return Content(xml.ToString(), "text/xml");
        }
    }
}
