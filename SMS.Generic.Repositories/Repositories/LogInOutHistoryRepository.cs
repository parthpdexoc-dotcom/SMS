using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Login/logout history-specific repository implementation.
    /// </summary>
    public class LogInOutHistoryRepository : GenericRepository<LogInOutHistory, SMSDbContext>, ILogInOutHistoryRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogInOutHistoryRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public LogInOutHistoryRepository(
            SMSDbContext context,
            ILogger<LogInOutHistoryRepository> logger) : base(context, logger)
        {
        }
    }
}

