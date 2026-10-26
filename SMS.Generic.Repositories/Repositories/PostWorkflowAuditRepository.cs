using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PostWorkflowAuditRepository : GenericRepository<PostWorkflowAudit, AuditDbContext>, IPostWorkflowAuditRepository
{
    public PostWorkflowAuditRepository(AuditDbContext context, ILogger<PostWorkflowAuditRepository> logger)
        : base(context, logger)
    {
    }
}
