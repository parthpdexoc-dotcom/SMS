using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class ClosedPostPositionAuditRepository : GenericRepository<ClosedPostPositionAudit, AuditDbContext>, IClosedPostPositionAuditRepository
{
    public ClosedPostPositionAuditRepository(AuditDbContext context, ILogger<ClosedPostPositionAuditRepository> logger)
        : base(context, logger)
    {
    }
}
