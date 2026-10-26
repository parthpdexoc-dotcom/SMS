namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Represents a payroll data record from database
/// </summary>
public class PayrollDataRecordDto
{
    /// <summary>When true, <see cref="GetExportEmployeeNumber"/> uses placeholder payroll rules (<c>9900</c> prefix).</summary>
    public bool IsPlaceholder { get; set; }

    public string EmployeeNumber { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public double Duration { get; set; }
    public string PayCode { get; set; } = string.Empty;
    public string? Project { get; set; }
    public string? Task { get; set; }
    public string? Award { get; set; }
    public string? Fund { get; set; }
    public Guid DepartmentId { get; set; }

    /// <summary>
    /// Folder name under DEPARTMENTS for employee imports (e.g. JR001); used for payroll export path Export\Payroll.
    /// </summary>
    public string? DepartmentExcelFolderName { get; set; }

    /// <summary>
    /// Employee number formatted for payroll export (regular: zero-padded; placeholder: <c>9900</c> + suffix or <c>990000000</c>).
    /// </summary>
    public string GetExportEmployeeNumber()
    {
        if (IsPlaceholder)
            return PlaceholderDto.FormatEmployeeNumberForPayroll(
                string.IsNullOrWhiteSpace(EmployeeNumber) ? null : EmployeeNumber);

        if (string.IsNullOrEmpty(EmployeeNumber))
            return EmployeeNumber;

        return EmployeeNumber.PadLeft(EmployeeNumber.Length + 4, '0');
    }
}
