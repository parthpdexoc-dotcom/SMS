using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

/// <summary>
/// Repository for <see cref="DepartmentCronJobManagement" />.
/// </summary>
public class DepartmentCronJobManagementRepository
    : GenericRepository<DepartmentCronJobManagement, SMSDbContext>, IDepartmentCronJobManagementRepository
{
    private readonly SMSDbContext _db;

    public DepartmentCronJobManagementRepository(SMSDbContext context,
        ILogger<DepartmentCronJobManagementRepository> logger)
        : base(context, logger)
    {
        _db = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<DepartmentCronJobScheduleDto>> GetDepartmentCronJobSchedulesByJobTypeAsync(
        JobType jobType,
        CancellationToken cancellationToken = default)
    {
        return await _db.Department
         .AsNoTracking()
         .Where(d => d.Active == true)
         .Select(d => new DepartmentCronJobScheduleDto
         {
             DepartmentId = d.DepartmentID,
             JobType = jobType,
             TimeZoneId = d.TimeZoneId,
             CronExpression = _db.DepartmentCronJobManagement
                 .Where(x =>
                     x.DepartmentID == d.DepartmentID &&
                     x.Job == jobType)
                 .Select(x => x.CronExpression)
                 .FirstOrDefault()
         })
         .ToListAsync(cancellationToken);
    }
}
