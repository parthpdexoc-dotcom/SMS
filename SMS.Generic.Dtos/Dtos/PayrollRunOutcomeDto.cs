using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>Result of one payroll generation attempt.</summary>
public readonly record struct PayrollRunOutcomeDto(PayrollExportRunStatus Status, string? FilePath, List<string>? Errors)
{
    public bool IsFinished => Status != PayrollExportRunStatus.Cancelled;

    public static PayrollRunOutcomeDto Completed(string? filePath) => new(PayrollExportRunStatus.Completed, filePath, null);

    public static PayrollRunOutcomeDto NoRecords() => new(PayrollExportRunStatus.NoRecords, null, null);

    public static PayrollRunOutcomeDto Failed(List<string>? errors) => new(PayrollExportRunStatus.Failed, null, errors);

    public static PayrollRunOutcomeDto Cancelled() => new(PayrollExportRunStatus.Cancelled, null, null);
}
