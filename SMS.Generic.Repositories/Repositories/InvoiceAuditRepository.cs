using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class InvoiceAuditRepository : GenericRepository<InvoiceAudit, AuditDbContext>, IInvoiceAuditRepository
{
    public InvoiceAuditRepository(AuditDbContext context, ILogger<InvoiceAuditRepository> logger)
        : base(context, logger)
    {
    }
}
