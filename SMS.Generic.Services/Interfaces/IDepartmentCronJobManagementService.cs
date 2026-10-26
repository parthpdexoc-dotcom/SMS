using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces;

/// <summary>Department admin UI for per-department cron schedules; does not change how jobs are hosted.</summary>
public interface IDepartmentCronJobManagementService
{
    /// <summary>
    /// Schedules for jobs that are actually configured (saved) for the department. Empty if none. Caller must be allowed to use the department.
    /// </summary>
    Task<IReadOnlyList<DepartmentCronScheduleDto>> GetSchedulesByDepartmentIdAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>Job types that have no <c>DepartmentCronJobManagement</c> row yet for this department.</summary>
    Task<IReadOnlyList<DepartmentCronJobTypeOptionDto>> GetUnconfiguredJobTypeOptionsForDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default);

    /// <summary>Creates or updates a single (department, job) row.</summary>
    Task<DepartmentCronScheduleDto> UpsertScheduleAsync(
        DepartmentCronScheduleUpsertDto dto,
        CancellationToken cancellationToken = default);
}
