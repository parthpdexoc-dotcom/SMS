using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class SkillAuditRepository : GenericRepository<SkillAudit, AuditDbContext>, ISkillAuditRepository
{
    public SkillAuditRepository(AuditDbContext context, ILogger<SkillAuditRepository> logger)
        : base(context, logger)
    {
    }
}
