using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requester locations-specific repository implementation.
    /// </summary>
    public class RequesterLocationsRepository : GenericRepository<RequesterLocations, SMSDbContext>, IRequesterLocationsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterLocationsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterLocationsRepository(
            SMSDbContext context,
            ILogger<RequesterLocationsRepository> logger) : base(context, logger)
        {
        }
    }
}

