using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class RequestAuditRepository : GenericRepository<RequestAudit, AuditDbContext>, IRequestAuditRepository
{
    public RequestAuditRepository(AuditDbContext context, ILogger<RequestAuditRepository> logger)
        : base(context, logger)
    {
    }
}
