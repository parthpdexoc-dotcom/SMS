using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class DepartmentActivityTypeAuditRepository : GenericRepository<DepartmentActivityTypeAudit, AuditDbContext>, IDepartmentActivityTypeAuditRepository
{
    public DepartmentActivityTypeAuditRepository(AuditDbContext context, ILogger<DepartmentActivityTypeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
