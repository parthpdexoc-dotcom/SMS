using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requester contacts-specific repository implementation.
    /// </summary>
    public class RequesterContactsRepository : GenericRepository<RequesterContacts, SMSDbContext>, IRequesterContactsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterContactsRepository(
            SMSDbContext context,
            ILogger<RequesterContactsRepository> logger) : base(context, logger)
        {
        }
    }
}

