using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class DepartmentDivisionsAuditRepository : GenericRepository<DepartmentDivisionsAudit, AuditDbContext>, IDepartmentDivisionsAuditRepository
{
    public DepartmentDivisionsAuditRepository(AuditDbContext context, ILogger<DepartmentDivisionsAuditRepository> logger)
        : base(context, logger)
    {
    }
}
