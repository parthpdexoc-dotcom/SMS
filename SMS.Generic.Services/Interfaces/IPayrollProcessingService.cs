using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces;

public interface IPayrollProcessingService
{
    /// <param name="departmentId">When set, only rows for this department are exported; otherwise all departments.</param>
    Task<PayrollProcessingResultDto> ProcessPayrollDataAsync(
        DateOnly startDate,
        DateOnly endDate,
        string? customFileName = null,
        Guid? departmentId = null);

    Task<PayrollProcessingResultDto> ProcessTodayPayrollDataAsync(
        string? customFileName = null);

    /// <param name="departmentId">When set (e.g. from department cron schedule), only that department’s payroll file is generated.</param>
    Task<PayrollProcessingResultDto> ProcessYesterdayPayrollDataAsync(
        DateOnly yesterday,
        DateOnly today,
        string? customFileName = null,
        Guid? departmentId = null);

    /// <summary>
    /// Most recent exported payroll CSV for <paramref name="departmentId"/> that was written at or after
    /// <paramref name="generatedOnOrAfterUtc"/>, or <see langword="null"/> when no such file exists.
    /// Lets callers detect an export that already ran without re-generating it.
    /// </summary>
    /// <param name="departmentFolderName">Department export folder; falls back to the department id when empty.</param>
    string? FindExportedPayrollFile(
        Guid departmentId,
        string? departmentFolderName,
        DateTime generatedOnOrAfterUtc);

    Task<ConnectionTestResultDto> TestConnectionsAsync();

    Task<Dictionary<Guid, int>> TestDepartmentDataRetrievalAsync(
        DateOnly startDate,
        DateOnly endDate);

    Task<List<string>> TestDepartmentDocumentGenerationAsync(
        DateOnly startDate,
        DateOnly endDate);
}
