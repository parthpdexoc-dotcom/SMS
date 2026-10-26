using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class DepartmentFeeAuditRepository : GenericRepository<DepartmentFeeAudit, AuditDbContext>, IDepartmentFeeAuditRepository
{
    public DepartmentFeeAuditRepository(AuditDbContext context, ILogger<DepartmentFeeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
