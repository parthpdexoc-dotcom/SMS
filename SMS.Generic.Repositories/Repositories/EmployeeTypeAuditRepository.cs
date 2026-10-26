using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class EmployeeTypeAuditRepository : GenericRepository<EmployeeTypeAudit, AuditDbContext>, IEmployeeTypeAuditRepository
{
    public EmployeeTypeAuditRepository(AuditDbContext context, ILogger<EmployeeTypeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
