using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PostAuditRepository : GenericRepository<PostAudit, AuditDbContext>, IPostAuditRepository
{
    public PostAuditRepository(AuditDbContext context, ILogger<PostAuditRepository> logger)
        : base(context, logger)
    {
    }
}
