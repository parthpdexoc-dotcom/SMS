using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requester type-specific repository implementation.
    /// </summary>
    public class RequesterTypeRepository : GenericRepository<RequesterType, SMSDbContext>, IRequesterTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterTypeRepository(
            SMSDbContext context,
            ILogger<RequesterTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

