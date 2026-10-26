using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Eligible type-specific repository implementation.
    /// </summary>
    public class EligibleTypeRepository : GenericRepository<EligibleType, SMSDbContext>, IEligibleTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EligibleTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EligibleTypeRepository(
            SMSDbContext context,
            ILogger<EligibleTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

