using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class EmailTemplateAuditRepository : GenericRepository<EmailTemplateAudit, AuditDbContext>, IEmailTemplateAuditRepository
{
    public EmailTemplateAuditRepository(AuditDbContext context, ILogger<EmailTemplateAuditRepository> logger)
        : base(context, logger)
    {
    }
}
