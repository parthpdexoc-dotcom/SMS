using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class BusinessTypeAuditRepository : GenericRepository<BusinessTypeAudit, AuditDbContext>, IBusinessTypeAuditRepository
{
    public BusinessTypeAuditRepository(AuditDbContext context, ILogger<BusinessTypeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
