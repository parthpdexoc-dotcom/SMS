using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Payment type-specific repository implementation.
    /// </summary>
    public class PaymentTypeRepository : GenericRepository<PaymentType, SMSDbContext>, IPaymentTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PaymentTypeRepository(
            SMSDbContext context,
            ILogger<PaymentTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

