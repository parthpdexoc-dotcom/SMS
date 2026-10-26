using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class EmployeeAuditRepository : GenericRepository<EmployeeAudit, AuditDbContext>, IEmployeeAuditRepository
{
    public EmployeeAuditRepository(AuditDbContext context, ILogger<EmployeeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
