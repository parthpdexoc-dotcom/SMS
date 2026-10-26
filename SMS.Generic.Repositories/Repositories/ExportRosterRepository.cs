using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Export roster-specific repository implementation.
    /// </summary>
    public class ExportRosterRepository : GenericRepository<ExportRoster, SMSDbContext>, IExportRosterRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportRosterRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ExportRosterRepository(
            SMSDbContext context,
            ILogger<ExportRosterRepository> logger) : base(context, logger)
        {
        }
    }
}

