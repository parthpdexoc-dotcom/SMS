using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requesters-specific repository implementation.
    /// </summary>
    public class RequesterRepository : GenericRepository<Requester, SMSDbContext>, IRequesterRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterRepository(
            SMSDbContext context,
            ILogger<RequesterRepository> logger) : base(context, logger)
        {
        }
    }
}

