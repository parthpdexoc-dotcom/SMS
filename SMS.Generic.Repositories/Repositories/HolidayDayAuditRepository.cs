using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class HolidayDayAuditRepository : GenericRepository<HolidayDayAudit, AuditDbContext>, IHolidayDayAuditRepository
{
    public HolidayDayAuditRepository(AuditDbContext context, ILogger<HolidayDayAuditRepository> logger)
        : base(context, logger)
    {
    }
}
