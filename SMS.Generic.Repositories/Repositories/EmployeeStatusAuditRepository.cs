using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class EmployeeStatusAuditRepository : GenericRepository<EmployeeStatusAudit, AuditDbContext>, IEmployeeStatusAuditRepository
{
    public EmployeeStatusAuditRepository(AuditDbContext context, ILogger<EmployeeStatusAuditRepository> logger)
        : base(context, logger)
    {
    }
}
