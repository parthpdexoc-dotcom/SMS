using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// HR status-specific repository implementation.
    /// </summary>
    public class HRStatusRepository : GenericRepository<HRStatus, SMSDbContext>, IHRStatusRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HRStatusRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public HRStatusRepository(
            SMSDbContext context,
            ILogger<HRStatusRepository> logger) : base(context, logger)
        {
        }
    }
}

