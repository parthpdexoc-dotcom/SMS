namespace SMS.Generic.Enums;

/// <summary>
/// Outcome of one scheduled payroll export attempt.
/// Distinct from <see cref="PayrollRunStatus"/>, which is used only for notification emails.
/// </summary>
public enum PayrollExportRunStatus
{
    Completed = 0,
    NoRecords = 1,
    Failed = 2,
    Cancelled = 3
}
