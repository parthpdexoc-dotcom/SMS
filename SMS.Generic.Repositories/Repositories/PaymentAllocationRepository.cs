using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for PaymentAllocation operations.
    /// </summary>
    public class PaymentAllocationRepository : GenericRepository<PaymentAllocation, SMSDbContext>, IPaymentAllocationRepository
    {
        public PaymentAllocationRepository(
            SMSDbContext context,
            ILogger<PaymentAllocationRepository> logger)
            : base(context, logger)
        {
        }
    }
}
