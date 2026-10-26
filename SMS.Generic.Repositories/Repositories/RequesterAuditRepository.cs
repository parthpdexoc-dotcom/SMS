using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

/// <summary>Audit database access for <see cref="RequesterAudit"/>.</summary>
public sealed class RequesterAuditRepository : GenericRepository<RequesterAudit, AuditDbContext>, IRequesterAuditRepository
{
    public RequesterAuditRepository(AuditDbContext context, ILogger<RequesterAuditRepository> logger)
        : base(context, logger)
    {
    }
}
