using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Request activity type-specific repository implementation.
    /// </summary>
    public class RequestActivityTypeRepository : GenericRepository<RequestActivityType, SMSDbContext>, IRequestActivityTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestActivityTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestActivityTypeRepository(
            SMSDbContext context,
            ILogger<RequestActivityTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

