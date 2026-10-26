using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requester configuration-specific repository implementation.
    /// </summary>
    public class RequesterConfigurationRepository : GenericRepository<RequesterConfiguration, SMSDbContext>, IRequesterConfigurationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigurationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterConfigurationRepository(
            SMSDbContext context,
            ILogger<RequesterConfigurationRepository> logger) : base(context, logger)
        {
        }
    }
}

