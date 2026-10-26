namespace SMS.Generic.Dtos.Dtos;

/// <summary>
/// One request and work-date group for tenure jobs dashboard grid.
/// </summary>
public class TenureJobsDashboardDto
{
    public TenureJobsDashboardDto()
    {
    }

    public TenureJobsDashboardDto(
        Guid requestId,
        string? requestTitle,
        string? projectNumber,
        int? fiscalYear,
        string? requesterName,
        DateOnly dateOfWork,
        DateOnly? processDate,
        TimeOnly? processTime,
        List<TenureJobsDashboardRowDto> tenureJobs)
    {
        RequestID = requestId;
        RequestTitle = requestTitle;
        RequestUniqueNumber = string.Concat(fiscalYear?.ToString(System.Globalization.CultureInfo.InvariantCulture), projectNumber);
        RequesterName = requesterName;
        DateOfWork = dateOfWork;
        ProcessDate = processDate;
        ProcessTime = processTime;
        TenureJobs = tenureJobs ?? new List<TenureJobsDashboardRowDto>();
    }

    public Guid RequestID { get; set; }
    public string? RequestTitle { get; set; }
    /// <summary>Fiscal year concatenated with project number (e.g. display key).</summary>
    public string? RequestUniqueNumber { get; set; }
    public string? RequesterName { get; set; }
    public DateOnly DateOfWork { get; set; }
    public DateOnly? ProcessDate { get; set; }
    public TimeOnly? ProcessTime { get; set; }
    public List<TenureJobsDashboardRowDto> TenureJobs { get; set; } = new();
}

/// <summary>
/// One tenure job row inside requester/date grouping.
/// </summary>
public class TenureJobsDashboardRowDto
{
    public TenureJobsDashboardRowDto()
    {
    }

    public TenureJobsDashboardRowDto(
        Guid postId,
        string? postTitle,
        DateOnly dateOfWork,
        TimeOnly startTime,
        TimeOnly endTime,
        int minimumHoursRequired,
        string? location,
        int totalPostPositions)
    {
        PostID = postId;
        PostTitle = postTitle;
        DateOfWork = dateOfWork;
        StartTime = startTime;
        EndTime = endTime;
        MinimumHoursRequired = minimumHoursRequired;
        Location = location;
        TotalPostPositions = totalPostPositions;
    }

    public Guid PostID { get; set; }
    public string? PostTitle { get; set; }
    public DateOnly DateOfWork { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int MinimumHoursRequired { get; set; }
    public string? Location { get; set; }
    public int TotalPostPositions { get; set; }
}
