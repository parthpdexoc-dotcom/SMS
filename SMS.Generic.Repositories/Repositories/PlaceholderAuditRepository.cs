using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PlaceholderAuditRepository : GenericRepository<PlaceholderAudit, AuditDbContext>, IPlaceholderAuditRepository
{
    public PlaceholderAuditRepository(AuditDbContext context, ILogger<PlaceholderAuditRepository> logger)
        : base(context, logger)
    {
    }
}
