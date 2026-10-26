using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PostPositionAuditRepository : GenericRepository<PostPositionAudit, AuditDbContext>, IPostPositionAuditRepository
{
    public PostPositionAuditRepository(AuditDbContext context, ILogger<PostPositionAuditRepository> logger)
        : base(context, logger)
    {
    }
}
