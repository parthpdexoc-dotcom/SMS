using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Requester contact types-specific repository implementation.
    /// </summary>
    public class RequesterContactTypesRepository : GenericRepository<RequesterContactTypes, SMSDbContext>, IRequesterContactTypesRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterContactTypesRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterContactTypesRepository(
            SMSDbContext context,
            ILogger<RequesterContactTypesRepository> logger) : base(context, logger)
        {
        }
    }
}

