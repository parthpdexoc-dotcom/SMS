namespace SMS.Generic.Dtos.Dtos
{
/// <summary>
/// One request and work-date group for open jobs dashboard grid.
/// </summary>
public class OpenJobsDashboardDto
{
    public OpenJobsDashboardDto()
    {
    }

    public OpenJobsDashboardDto(
        Guid requestId,
        string? requestTitle,
        string? projectNumber,
        int? fiscalYear,
        string? requesterName,
        DateOnly dateOfWork,
        List<OpenJobDashboardRowDto> openJobs,
        int totalPostPositions,
        int totalOpen,
        int totalFilled,
        int totalCancel)
    {
        RequestID = requestId;
        RequestTitle = requestTitle;
        RequestUniqueNumber = string.Concat(fiscalYear?.ToString(System.Globalization.CultureInfo.InvariantCulture), projectNumber);
        RequesterName = requesterName;
        DateOfWork = dateOfWork;
        OpenJobs = openJobs ?? new List<OpenJobDashboardRowDto>();
        TotalPositions = totalPostPositions;
        TotalOpen = totalOpen;
        TotalFilled = totalFilled;
        TotalCancel = totalCancel;
    }

    public Guid RequestID { get; set; }
    public string? RequestTitle { get; set; }
    /// <summary>Fiscal year concatenated with project number (e.g. display key).</summary>
    public string? RequestUniqueNumber { get; set; }
    public string? RequesterName { get; set; }
    public DateOnly DateOfWork { get; set; }
    public int TotalPositions { get; set; }
    public int TotalOpen { get; set; }
    public int TotalFilled { get; set; }
    public int TotalCancel { get; set; }
    public List<OpenJobDashboardRowDto> OpenJobs { get; set; } = new();
}

/// <summary>
/// One dashboard row: request + post + work date; slot counts match available-post booking rules.
/// </summary>
public class OpenJobDashboardRowDto
{
    public Guid PostID { get; set; }
    public string PostTitle { get; set; } = string.Empty;

    public int TotalPostPositions { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    /// <summary>Unfilled slots (same as available-post &quot;open&quot; count for that date).</summary>
    public int Open { get; set; }

    /// <summary>Staffed slots excluding those in cancellation pending.</summary>
    public int Filled { get; set; }

    public int PendingCancellation { get; set; }
}
}
