using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class RequestScheduleAuditRepository : GenericRepository<RequestScheduleAudit, AuditDbContext>, IRequestScheduleAuditRepository
{
    public RequestScheduleAuditRepository(AuditDbContext context, ILogger<RequestScheduleAuditRepository> logger)
        : base(context, logger)
    {
    }
}
