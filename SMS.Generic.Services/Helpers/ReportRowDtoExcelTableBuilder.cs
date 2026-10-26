using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Column order and cell values for exporting <see cref="ReportRowDto"/> rows to a flat table (Excel, CSV, etc.).
/// </summary>
public static class ReportRowDtoExcelTableBuilder
{
    public static IReadOnlyList<string> Headers { get; } =
    [
        "OfficerID",
        "Officer",
        "Payroll",
        "Pvt",
        "JobID",
        "ShiftID",
        "Hours Worked",
        "ShiftDay",
        "Pay Code",
        "Comments",
        "Employer Name",
        "Employer Address",
        "Start Time",
        "Hours Required",
        "Response Area",
        "Job Address",
        "Call Sign",
        "Vehicle",
        "On Duty",
        "Shift For",
        "Emp Hrly Rate",
        "Post Pay Rate",
        "LDR PC 116",
        "LDR PC 103",
        "LDR PC 102",
        "PC 102",
        "PC 102 Rate",
        "Veh #",
        "Veh Hrs",
        "Job Name",
        "Agency",
        "ClosedBy",
        "PTAEO",
        "Invoice",
        "Is Placeholder",
        "Is Closed",
        "Vehicle Closed Hours",
        "Personal Rank",
        "Shift Rank",
    ];

    public static IEnumerable<IReadOnlyList<object?>> ToDataRows(IEnumerable<ReportRowDto> rows) =>
        rows.Select(ToValues);

    public static IReadOnlyList<object?> ToValues(ReportRowDto r) =>
    [
        ReportExcelCellFormatHelper.AsPaddedCode(r.EmployeeCode),
        r.EmployeeName,
        ReportExcelCellFormatHelper.AsInteger(r.Payroll),
        r.Pvt,
        ReportExcelCellFormatHelper.AsPaddedCode(r.RequestCode),
        ReportExcelCellFormatHelper.AsPaddedCode(r.PostCode),
        ReportExcelCellFormatHelper.RoundTwo(r.HoursWorked),
        r.PostDay,
        ReportExcelCellFormatHelper.AsInteger(r.PayCode),
        r.Comments,
        r.RequesterName,
        r.RequesterAddress,
        ReportExcelCellFormatHelper.FormatTimeHhMm(r.StartTime),
        ReportExcelCellFormatHelper.RoundTwo(r.HoursRequired),
        r.ResponseArea,
        r.RequestAddress,
        ReportExcelCellFormatHelper.AsInteger(r.CallSign),
        r.Vehicle,
        r.OnDuty,
        r.PostFor,
        ReportExcelCellFormatHelper.RoundTwo(r.EmpHrlyRate),
        ReportExcelCellFormatHelper.RoundTwo(r.PostPayRate),
        ReportExcelCellFormatHelper.RoundTwo(r.LdrPc116),
        ReportExcelCellFormatHelper.RoundTwo(r.LdrPc103),
        ReportExcelCellFormatHelper.RoundTwo(r.LdrPc102),
        ReportExcelCellFormatHelper.RoundTwo(r.Pc102),
        ReportExcelCellFormatHelper.RoundTwo(r.Pc102Rate),
        ReportExcelCellFormatHelper.AsInteger(r.VehicleNumber),
        ReportExcelCellFormatHelper.RoundTwo(r.VehicleHrs),
        r.RequestName,
        r.Agency,
        r.ClosedBy,
        r.Ptaeo,
        r.Invoice,
        r.IsPlaceholder ? "Yes" : "No",
        r.IsClosed ? "Yes" : "No",
        ReportExcelCellFormatHelper.RoundTwo(r.VehicleClosedHours),
        r.PersonalRank,
        r.ShiftRank,
    ];
}
