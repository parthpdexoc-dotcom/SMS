using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PayCodeAuditRepository : GenericRepository<PayCodeAudit, AuditDbContext>, IPayCodeAuditRepository
{
    public PayCodeAuditRepository(AuditDbContext context, ILogger<PayCodeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
