using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Repositories.Interfaces;

/// <summary>
/// Repository for <see cref="DepartmentCronJobManagement"/> rows and related schedule queries.
/// </summary>
/// <remarks>Reads/filters/upserts use <see cref="IGenericRepository{DepartmentCronJobManagement}"/>
/// (e.g. <c>GetAllAsync</c>, <c>UpsertAsync</c>); see also the projection for background jobs below.</remarks>
public interface IDepartmentCronJobManagementRepository : IGenericRepository<DepartmentCronJobManagement>
{
    /// <summary>
    /// All department cron rows for the given job type; returns an empty list when the table has no rows, so the scheduler uses the default appsettings cron in UTC for that job only.
    /// </summary>
    Task<IReadOnlyList<DepartmentCronJobScheduleDto>> GetDepartmentCronJobSchedulesByJobTypeAsync(
        JobType jobType,
        CancellationToken cancellationToken = default);
}
