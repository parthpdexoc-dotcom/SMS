using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for PayCode operations.
    /// </summary>
    public class PayCodeRepository : GenericRepository<PayCode, SMSDbContext>, IPayCodeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayCodeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PayCodeRepository(
            SMSDbContext context,
            ILogger<PayCodeRepository> logger)
            : base(context, logger)
        {
        }
    }
}
