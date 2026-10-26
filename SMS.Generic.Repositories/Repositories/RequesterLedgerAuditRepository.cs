using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class RequesterLedgerAuditRepository : GenericRepository<RequesterLedgerAudit, AuditDbContext>, IRequesterLedgerAuditRepository
{
    public RequesterLedgerAuditRepository(AuditDbContext context, ILogger<RequesterLedgerAuditRepository> logger)
        : base(context, logger)
    {
    }
}
