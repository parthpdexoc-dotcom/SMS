using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class ClosedPostPositionEquipmentAuditRepository : GenericRepository<ClosedPostPositionEquipmentAudit, AuditDbContext>, IClosedPostPositionEquipmentAuditRepository
{
    public ClosedPostPositionEquipmentAuditRepository(AuditDbContext context, ILogger<ClosedPostPositionEquipmentAuditRepository> logger)
        : base(context, logger)
    {
    }
}
