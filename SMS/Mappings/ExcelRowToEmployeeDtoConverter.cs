using AutoMapper;
using System.Globalization;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Mappings;

/// <summary>
/// Converts an Excel row (Dictionary with keys like LAST_NAME, FIRST_NAME, EMPLOYEE_NUMBER, etc.) to EmployeeDto.
/// Only maps columns that have a direct DTO property. DepartmentID, EmployeeTypeID, StatusID, and DivisionID are resolved in EmployeeJob from row columns (DEPARTMENT, HR_STATUS, CURRENT_ASSIGNMENT, DIVISION) via service lookups. EmployeeID is set by EmployeeJob when updating existing employees.
/// </summary>
public class ExcelRowToEmployeeDtoConverter : ITypeConverter<Dictionary<string, string>, EmployeeDto>
{
    private static readonly StringComparison KeyComparison = StringComparison.OrdinalIgnoreCase;

    public EmployeeDto Convert(Dictionary<string, string> source, EmployeeDto destination, ResolutionContext context)
    {
        if (source == null)
            return destination ?? new EmployeeDto();

        var hourlySalary = (GetDecimal(source, "HOURLY_SALARY") is { } salary && salary > 0) ? salary : 0.01m;

        return new EmployeeDto
        {
            LastName = GetValue(source, "LAST_NAME"),
            FirstName = GetValue(source, "FIRST_NAME"),
            BadgeNumber = GetValue(source, "EMPLOYEE_NUMBER"),
            PrimaryEmail = GetValue(source, "EMAIL_ADDRESS"),
            PrimaryPhone = string.IsNullOrWhiteSpace(GetValue(source, "RETURN_TO_WORK1"))
            ? "727-555-1212": GetValue(source, "RETURN_TO_WORK1"),
            SecondaryPhone = string.IsNullOrWhiteSpace(GetValue(source, "RETURN_TO_WORK2"))
            ? "727-555-1212" : GetValue(source, "RETURN_TO_WORK2"),
            Username = GetValue(source, "ORA_USER_NAME"),
            HourlySalary = hourlySalary,
            Password = GetValue(source, "PASSWORD"),
            HrCostCenterCode = GetValue(source, "HR_COSTCENTER_CODE"),
            EmployeeNumber = GetValue(source, "EMPLOYEE_NUMBER"),
            EmployeeDepartmentDefinedField1 = GetValue(source, "PROBATION_INDICATOR"),
            EmployeeDepartmentDefinedField2 = GetValue(source, "CITY_CELL"),
            EmployeeDepartmentDefinedField3 = GetValue(source, "WORK_PHONE"),
            ImportedDateOfHire = GetDate(source, "HIRE_DATE"),
            ImportedDateOfPromotion = GetDate(source, "SENIORITY_DATE"),
            IsApproved = hourlySalary > 0
        };
    }

    private static string? GetValue(Dictionary<string, string> row, string key)
    {
        var pair = row.FirstOrDefault(p => string.Equals(p.Key, key, KeyComparison));
        return pair.Value?.Trim();
    }

    private static decimal? GetDecimal(Dictionary<string, string> row, string key)
    {
        var value = GetValue(row, key);
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
    }

    private static DateOnly? GetDate(Dictionary<string, string> row, string key)
    {
        var value = GetValue(row, key);
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var invariantDate))
            return invariantDate;

        if (DateOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var currentCultureDate))
            return currentCultureDate;

        return null;
    }
}
