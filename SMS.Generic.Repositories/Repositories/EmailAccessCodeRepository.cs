using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Email access code-specific repository implementation.
    /// </summary>
    public class EmailAccessCodeRepository : GenericRepository<EmailAccessCode, SMSDbContext>, IEmailAccessCodeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAccessCodeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailAccessCodeRepository(
            SMSDbContext context,
            ILogger<EmailAccessCodeRepository> logger) : base(context, logger)
        {
        }
    }
}

