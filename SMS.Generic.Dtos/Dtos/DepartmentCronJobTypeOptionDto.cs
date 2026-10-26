using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos;

/// <summary>Job type that can still get a <c>DepartmentCronJobManagement</c> row for a department (not yet configured).</summary>
public class DepartmentCronJobTypeOptionDto
{
    public JobType Job { get; set; }

    public string JobName { get; set; } = string.Empty;
}
