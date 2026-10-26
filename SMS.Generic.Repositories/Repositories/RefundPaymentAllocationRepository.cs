using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class RefundPaymentAllocationRepository : GenericRepository<RefundPaymentAllocation, SMSDbContext>, IRefundPaymentAllocationRepository
    {
        public RefundPaymentAllocationRepository(
            SMSDbContext context,
            ILogger<RefundPaymentAllocationRepository> logger)
            : base(context, logger)
        {
        }
    }
}
