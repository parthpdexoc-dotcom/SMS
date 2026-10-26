using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Email communication-specific repository implementation.
    /// </summary>
    public class EmailCommunicationRepository : GenericRepository<EmailCommunication, SMSDbContext>, IEmailCommunicationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailCommunicationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailCommunicationRepository(
            SMSDbContext context,
            ILogger<EmailCommunicationRepository> logger) : base(context, logger)
        {
        }
    }
}

