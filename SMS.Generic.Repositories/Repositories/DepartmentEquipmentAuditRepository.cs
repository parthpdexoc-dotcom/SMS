using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class DepartmentEquipmentAuditRepository : GenericRepository<DepartmentEquipmentAudit, AuditDbContext>, IDepartmentEquipmentAuditRepository
{
    public DepartmentEquipmentAuditRepository(AuditDbContext context, ILogger<DepartmentEquipmentAuditRepository> logger)
        : base(context, logger)
    {
    }
}
