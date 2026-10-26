using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class EquipmentTypeAuditRepository : GenericRepository<EquipmentTypeAudit, AuditDbContext>, IEquipmentTypeAuditRepository
{
    public EquipmentTypeAuditRepository(AuditDbContext context, ILogger<EquipmentTypeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
