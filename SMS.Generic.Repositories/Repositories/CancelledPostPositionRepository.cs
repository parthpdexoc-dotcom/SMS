using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for CancelledPostPosition entity operations.
    /// </summary>
    public class CancelledPostPositionRepository : GenericRepository<CancelledPostPosition, SMSDbContext>, ICancelledPostPositionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledPostPositionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public CancelledPostPositionRepository(
            SMSDbContext context,
            ILogger<CancelledPostPositionRepository> logger)
            : base(context, logger)
        {
        }
    }
}
