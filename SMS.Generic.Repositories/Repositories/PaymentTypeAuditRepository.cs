using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PaymentTypeAuditRepository : GenericRepository<PaymentTypeAudit, AuditDbContext>, IPaymentTypeAuditRepository
{
    public PaymentTypeAuditRepository(AuditDbContext context, ILogger<PaymentTypeAuditRepository> logger)
        : base(context, logger)
    {
    }
}
