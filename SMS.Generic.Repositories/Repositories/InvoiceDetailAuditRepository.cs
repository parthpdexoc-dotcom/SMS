using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class InvoiceDetailAuditRepository : GenericRepository<InvoiceDetailAudit, AuditDbContext>, IInvoiceDetailAuditRepository
{
    public InvoiceDetailAuditRepository(AuditDbContext context, ILogger<InvoiceDetailAuditRepository> logger)
        : base(context, logger)
    {
    }
}
