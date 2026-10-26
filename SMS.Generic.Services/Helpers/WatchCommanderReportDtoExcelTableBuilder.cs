using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Column order and cell values for exporting <see cref="WatchCommanderReportDto"/> rows to a flat table (Excel, etc.).
/// </summary>
public static class WatchCommanderReportDtoExcelTableBuilder
{
    public static IReadOnlyList<string> Headers { get; } =
    [
        "Date",
        "Officer Name",
        "Payroll",
        "Call Sign",
        "Officer Type",
        "Start Time",
        "End Time",
        "Hours",
        "Client",
        "Location",
        "Vehicle",
    ];

    public static IEnumerable<IReadOnlyList<object?>> ToDataRows(IEnumerable<WatchCommanderReportDto> rows) =>
        rows.Select(ToValues);

    public static IReadOnlyList<object?> ToValues(WatchCommanderReportDto r) =>
    [
        ReportExcelCellFormatHelper.FormatDateMmDdYy(r.StartDate),
        r.EmployeeName,
        r.Payroll,
        r.CallSign,
        r.EmployeeType,
        ReportExcelCellFormatHelper.FormatTimeHhMm(r.StartTime),
        ReportExcelCellFormatHelper.FormatTimeHhMm(r.EndTime),
        ReportExcelCellFormatHelper.FormatTrimmedDecimal(r.HoursWorked),
        r.Requester,
        r.RequesterAddress,
        r.Vehicle,
    ];
}
