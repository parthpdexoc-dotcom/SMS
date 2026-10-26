using System.Globalization;
using Microsoft.Extensions.Configuration;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Dtos.Extensions;
using SMS.Generic.Enums;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services;

public class PayrollNotificationService : IPayrollNotificationService
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<PayrollNotificationService> _logger;

    public PayrollNotificationService(
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<PayrollNotificationService> logger)
    {
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task NotifyAsync(
        PayrollProcessingResultDto result,
        string departmentName,
        PayrollRunStatus status,
        string? departmentTimeZoneId = null,
        DateTime? retryUntilUtc = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var toEmail = _configuration["PayrollSettings:Notifications:SupportEmail"];
            if (string.IsNullOrWhiteSpace(toEmail))
                return;

            var ccAddresses = ReadRecipients("CcEmails");

            // Monitoring copy: stripped from the delivered message, so the support recipient never sees it.
            var bccAddresses = ReadRecipients("BccEmails");

            var reason = string.Join("; ", result.Errors.Where(e => !string.IsNullOrWhiteSpace(e)));
            if (string.IsNullOrWhiteSpace(reason))
                reason = "Unknown error";

            string subject;
            string body;

            switch (status)
            {
                case PayrollRunStatus.Success:
                {
                    var hasRecords = result.PayrollDataRecords.Any();
                    subject = hasRecords
                        ? $"SUCCESS - PAYROLL PROCESSING {departmentName}"
                        : $"NO RECORDS - PAYROLL PROCESSING {departmentName}";
                    var message = hasRecords
                        ? $"{result.PayrollDataRecords.Count} number of records exported successfully."
                        : "No closed payroll records were found. No export file was generated.";
                    body = $"Department Name: {departmentName}{Environment.NewLine}Date Range: {result.DateRange}{Environment.NewLine}{message}";
                    break;
                }
                case PayrollRunStatus.Retry:
                {
                    if (!retryUntilUtc.HasValue)
                        return;

                    var deadlineLocal = TimeZoneInfo.ConvertTimeFromUtc(retryUntilUtc.Value, departmentTimeZoneId.ToSafeTimeZone());
                    var deadlineText = deadlineLocal.ToString("h:mm tt", CultureInfo.InvariantCulture).ToLowerInvariant();
                    subject = $"FAILURE-RETRYING PAYROLL PROCESSING {departmentName}";
                    body = $"Department Name: {departmentName}{Environment.NewLine}Reason: {reason}{Environment.NewLine}Will continue retrying until {deadlineText}.";
                    break;
                }
                case PayrollRunStatus.Failed:
                    subject = $"FAILED - PAYROLL PROCESSING {departmentName}";
                    body = $"Department Name: {departmentName}{Environment.NewLine}FAILED-REASON - export failed due to {reason}.";
                    break;
                default:
                    return;
            }

            await _emailSender.SendAsync(
                new[] { toEmail.Trim() },
                subject,
                body,
                isBodyHtml: false,
                ccAddresses: ccAddresses.Length > 0 ? ccAddresses : null,
                bccAddresses: bccAddresses.Length > 0 ? bccAddresses : null,
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Payroll export email failed for department {DepartmentName} ({Status}).", departmentName, status);
        }
    }

    private string[] ReadRecipients(string key)
        => (_configuration.GetSection($"PayrollSettings:Notifications:{key}").Get<string[]>() ?? Array.Empty<string>())
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e.Trim())
            .ToArray();
}
