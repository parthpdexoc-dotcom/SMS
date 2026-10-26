using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Export log-specific repository implementation.
    /// </summary>
    public class ExportLogRepository : GenericRepository<ExportLog, SMSDbContext>, IExportLogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportLogRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ExportLogRepository(
            SMSDbContext context,
            ILogger<ExportLogRepository> logger) : base(context, logger)
        {
        }
    }
}

