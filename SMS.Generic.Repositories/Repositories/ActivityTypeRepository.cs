using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Activity type-specific repository implementation.
    /// </summary>
    public class ActivityTypeRepository : GenericRepository<ActivityType, SMSDbContext>, IActivityTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ActivityTypeRepository(
            SMSDbContext context,
            ILogger<ActivityTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

