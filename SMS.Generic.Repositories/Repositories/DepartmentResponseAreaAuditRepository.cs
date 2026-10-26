using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class DepartmentResponseAreaAuditRepository : GenericRepository<DepartmentResponseAreaAudit, AuditDbContext>, IDepartmentResponseAreaAuditRepository
{
    public DepartmentResponseAreaAuditRepository(AuditDbContext context, ILogger<DepartmentResponseAreaAuditRepository> logger)
        : base(context, logger)
    {
    }
}
