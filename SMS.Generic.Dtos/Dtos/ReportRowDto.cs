namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// Flat row for operator activity / payroll-style reports (no backing entity; projection-only).
/// </summary>
public class ReportRowDto
{
    private static readonly decimal[] AllowedPc102PostRates = [60m, 65m, 70m];

    /// <summary>
    /// For model binding / deserialization.
    /// </summary>
    public ReportRowDto()
    {
    }

    /// <summary>
    /// Builds a report row from closed-post projection data.
    /// </summary>
    public ReportRowDto(
        string? employeeCode,
        string? employeeName,
        string? payroll,
        string? pvt,
        string? requestCode,
        string? postCode,
        decimal? hoursWorked,
        DateOnly? postDay,
        string? payCode,
        string? comments,
        string? requesterName,
        string? requesterAddress,
        TimeOnly? startTime,
        decimal? hoursRequired,
        string? responseArea,
        string? requestAddress,
        string? callSign,
        string? vehicle,
        bool? onDuty,
        string? postFor,
        decimal? empHrlyRate,
        decimal? postPayRate,
        string? vehicleNumber,
        decimal? vehicleHrs,
        string? requestName,
        string? agency,
        string? closedBy,
        string? ptaeo = null,
        string? invoice = null,
        bool isPlaceholder = false,
        bool isClosed = false,
        decimal? vehicleClosedHours = null,
        string? personalRank = null,
        string? shiftRank = null)
    {
        EmployeeCode = employeeCode;
        EmployeeName = employeeName;
        Payroll = payroll;
        Pvt = pvt;
        RequestCode = requestCode;
        PostCode = postCode;
        HoursWorked = hoursWorked;
        PostDay = postDay;
        PayCode = payCode;
        Comments = comments;
        RequesterName = requesterName;
        RequesterAddress = requesterAddress;
        StartTime = startTime;
        HoursRequired = hoursRequired;
        ResponseArea = responseArea;
        RequestAddress = requestAddress;
        CallSign = callSign;
        Vehicle = vehicle;
        OnDuty = onDuty;
        PostFor = postFor;
        EmpHrlyRate = empHrlyRate;
        PostPayRate = postPayRate;
        LdrPc116 = CalculateLdrPc116(payCode, hoursWorked, empHrlyRate);
        LdrPc103 = CalculateLdrPc103(payCode, hoursWorked, empHrlyRate);
        LdrPc102 = CalculateLdrPc102(payCode, hoursWorked, empHrlyRate);
        Pc102 = CalculatePc102(payCode, hoursWorked, postPayRate);
        Pc102Rate = GetPc102PostRate(payCode, postPayRate);
        VehicleNumber = vehicleNumber;
        VehicleHrs = vehicleHrs;
        RequestName = requestName;
        Agency = agency;
        ClosedBy = closedBy;
        Ptaeo = ptaeo;
        Invoice = invoice;
        IsPlaceholder = isPlaceholder;
        IsClosed = isClosed;
        VehicleClosedHours = vehicleClosedHours;
        PersonalRank = personalRank;
        ShiftRank = shiftRank;
    }

    /// <summary>Business code for the officer (archived report &quot;OfficerID&quot; column).</summary>
    public string? EmployeeCode { get; set; }

    public string? EmployeeName { get; set; }

    public string? Payroll { get; set; }

    /// <summary>Employee off-duty call back phone number (archived report &quot;Pvt&quot; column).</summary>
    public string? Pvt { get; set; }

    /// <summary>Request business code (archived report &quot;JobID&quot; column).</summary>
    public string? RequestCode { get; set; }

    /// <summary>Post business code (archived report &quot;ShiftID&quot; column).</summary>
    public string? PostCode { get; set; }

    public decimal? HoursWorked { get; set; }

    public DateOnly? PostDay { get; set; }

    public string? PayCode { get; set; }

    public string? Comments { get; set; }

    public string? RequesterName { get; set; }

    public string? RequesterAddress { get; set; }

    public TimeOnly? StartTime { get; set; }

    public decimal? HoursRequired { get; set; }

    public string? ResponseArea { get; set; }

    public string? RequestAddress { get; set; }

    public string? CallSign { get; set; }

    public string? Vehicle { get; set; }

    public bool? OnDuty { get; set; }

    public string? PostFor { get; set; }

    /// <summary>Employee hourly salary from the employee record.</summary>
    public decimal? EmpHrlyRate { get; set; }

    /// <summary>Post pay rate from the post record.</summary>
    public decimal? PostPayRate { get; set; }

    public decimal? LdrPc116 { get; set; }

    public decimal? LdrPc103 { get; set; }

    public decimal? LdrPc102 { get; set; }

    /// <summary>PC 102: pay code 102 jobs with post pay rate $60, $65, or $70 — hours worked × post pay rate.</summary>
    public decimal? Pc102 { get; set; }

    /// <summary>Pay code 102 jobs: displays post pay rate when the post rate is $60, $65, or $70.</summary>
    public decimal? Pc102Rate { get; set; }

    public string? VehicleNumber { get; set; }

    public decimal? VehicleHrs { get; set; }

    public string? RequestName { get; set; }

    public string? Agency { get; set; }

    public string? ClosedBy { get; set; }

    public string? Ptaeo { get; set; }

    public string? Invoice { get; set; }

    public bool IsPlaceholder { get; set; }

    public bool IsClosed { get; set; }

    public decimal? VehicleClosedHours { get; set; }

    public string? PersonalRank { get; set; }

    public string? ShiftRank { get; set; }

    private static decimal? CalculateLdrPc116(string? payCode, decimal? hoursWorked, decimal? empHrlyRate) =>
        IsPayCodeNumber(payCode, "116") && hoursWorked.HasValue && empHrlyRate.HasValue
            ? RoundTwo(hoursWorked.Value * empHrlyRate.Value * 1.25m)
            : null;

    private static decimal? CalculateLdrPc103(string? payCode, decimal? hoursWorked, decimal? empHrlyRate) =>
        IsPayCodeNumber(payCode, "103") && hoursWorked.HasValue && empHrlyRate.HasValue
            ? RoundTwo(hoursWorked.Value * empHrlyRate.Value * 1.50m)
            : null;

    private static decimal? CalculateLdrPc102(string? payCode, decimal? hoursWorked, decimal? empHrlyRate) =>
        IsPayCodeNumber(payCode, "102") && hoursWorked.HasValue && empHrlyRate.HasValue
            ? RoundTwo(hoursWorked.Value * empHrlyRate.Value * 1.25m)
            : null;

    private static decimal? CalculatePc102(string? payCode, decimal? hoursWorked, decimal? postPayRate) =>
        hoursWorked.HasValue && postPayRate.HasValue && IsPayCode102WithAllowedPostRate(payCode, postPayRate.Value)
            ? RoundTwo(hoursWorked.Value * postPayRate.Value)
            : null;

    private static decimal RoundTwo(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private static decimal? GetPc102PostRate(string? payCode, decimal? postPayRate) =>
        postPayRate.HasValue && IsPayCode102WithAllowedPostRate(payCode, postPayRate.Value) ? postPayRate : null;

    private static bool IsPayCode102WithAllowedPostRate(string? payCode, decimal postPayRate) =>
        IsPayCodeNumber(payCode, "102") && IsAllowedPc102PostRate(postPayRate);

    private static bool IsPayCodeNumber(string? payCode, string number) =>
        payCode != null && string.Equals(payCode.Trim(), number, StringComparison.OrdinalIgnoreCase);

    private static bool IsAllowedPc102PostRate(decimal postPayRate) =>
        AllowedPc102PostRates.Contains(postPayRate);
}
