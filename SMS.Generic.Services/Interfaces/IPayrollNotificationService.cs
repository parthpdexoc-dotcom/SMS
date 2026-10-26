using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

public interface IPayrollNotificationService
{
    Task NotifyAsync(
        PayrollProcessingResultDto result,
        string departmentName,
        PayrollRunStatus status,
        string? departmentTimeZoneId = null,
        DateTime? retryUntilUtc = null,
        CancellationToken cancellationToken = default);
}
