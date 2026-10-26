using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class RoutingGroupAuditRepository : GenericRepository<RoutingGroupAudit, AuditDbContext>, IRoutingGroupAuditRepository
{
    public RoutingGroupAuditRepository(AuditDbContext context, ILogger<RoutingGroupAuditRepository> logger)
        : base(context, logger)
    {
    }
}
