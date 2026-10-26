using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities.Audit;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories;

public sealed class PaymentAllocationAuditRepository : GenericRepository<PaymentAllocationAudit, AuditDbContext>, IPaymentAllocationAuditRepository
{
    public PaymentAllocationAuditRepository(AuditDbContext context, ILogger<PaymentAllocationAuditRepository> logger)
        : base(context, logger)
    {
    }
}
