using System.Globalization;
using SMS.Generic.Dtos.Dtos;
using static SMS.Generic.Dtos.Dtos.PostDto;
using static SMS.Generic.Dtos.Dtos.RosterDto;

namespace SMS.Generic.Services.Helpers;

/// <summary>
/// Maps roster information DTOs to the department roster export layout.
/// </summary>
public static class RosterInformationExcelBuilder
{
    public const string SheetName = "Roster";
    public static readonly IReadOnlyList<string> ColumnHeaders =
        ["Unit", "Officer", "Payroll", "Vehicle", "Assignment"];

    public static string BuildFileName(string? requesterName, DateOnly dateOfWork)
    {
        var requester = SanitizeFileNamePart(requesterName, "REQUESTER");
        return $"Roster {requester} {dateOfWork:yyyy-MM-dd}.xlsx";
    }

    public static string BuildTitleLine(string? requesterName)
        => string.IsNullOrWhiteSpace(requesterName) ? string.Empty : requesterName.Trim();

    public static IReadOnlyList<object?> ToDataRow(RosterInformationDto row)
    {
        var assignment = DisplayValue(row.PostLocation);
        return
        [
            DisplayValue(row.CallUnit),
            DisplayValue(row.EmployeeName),
            DisplayValue(row.EmployeeNumber),
            DisplayValue(row.VehicleName),
            assignment,
        ];
    }

    public static string DisplayValue(string? value)
        => string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();

    private static string SanitizeFileNamePart(string? value, string fallback)
    {
        var part = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            part = part.Replace(c, '_');
        }

        return part;
    }
}
