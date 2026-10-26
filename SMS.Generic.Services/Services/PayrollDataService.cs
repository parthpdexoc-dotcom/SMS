using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Repositories.Interfaces;
using SMS.Generic.Services.Interfaces;

namespace SMS.Generic.Services.Services;

/// <summary>
/// Service to retrieve payroll data using repository pattern
/// </summary>
public class PayrollDataService : IPayrollDataService
{
    private readonly ILogger<PayrollDataService> _logger;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IConfiguration _configuration;

    public PayrollDataService(
        ILogger<PayrollDataService> logger,
        IPayrollRepository payrollRepository,
        IConfiguration configuration)
    {
        _logger = logger;
        _payrollRepository = payrollRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Gets payroll data from start date through today
    /// </summary>
    /// <param name="startDate">Start date for payroll data</param>
    /// <returns>List of payroll records</returns>
    public async Task<List<PayrollDataRecordDto>> GetPayrollDataAsync(DateOnly startDate, DateOnly endDate, Guid? departmentId = null)
    {
        try
        {
            _logger.LogInformation(
                "Retrieving payroll data from {StartDate} to {EndDate}{Dept}",
                startDate,
                endDate,
                departmentId.HasValue && departmentId.Value != Guid.Empty ? $", department {departmentId.Value}" : string.Empty);

            var payrollCloseTimeSetting = _configuration["PayrollSettings:CloseWindowTime"] ?? "08:30";
            var hasValidCloseWindowTime = TimeOnly.TryParse(payrollCloseTimeSetting, out var parsedCloseWindowTime);
            var closeWindowTime = hasValidCloseWindowTime ? parsedCloseWindowTime : new TimeOnly(8, 30);

            if (!hasValidCloseWindowTime)
            {
                _logger.LogWarning(
                    "Invalid PayrollSettings:CloseWindowTime value '{CloseWindowTimeSetting}'. Falling back to 08:30.",
                    payrollCloseTimeSetting);
            }

            var closeWindowStart = startDate.ToDateTime(closeWindowTime);
            var closeWindowEnd = endDate.ToDateTime(closeWindowTime);

            var records = await _payrollRepository.GetPayrollDataAsync(closeWindowStart, closeWindowEnd, departmentId);
            
            _logger.LogInformation("Retrieved {Count} payroll records", records.Count);
            return records;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payroll data");
            throw;
        }
    }
}
