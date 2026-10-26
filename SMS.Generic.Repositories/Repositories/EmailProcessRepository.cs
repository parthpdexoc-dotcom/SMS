using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Email process-specific repository implementation.
    /// </summary>
    public class EmailProcessRepository : GenericRepository<EmailProcess, SMSDbContext>, IEmailProcessRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailProcessRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailProcessRepository(
            SMSDbContext context,
            ILogger<EmailProcessRepository> logger) : base(context, logger)
        {
        }
    }
}

