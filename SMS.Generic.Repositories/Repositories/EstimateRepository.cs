using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Estimate-specific repository implementation.
    /// </summary>
    public class EstimateRepository : GenericRepository<Estimate, SMSDbContext>, IEstimateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EstimateRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EstimateRepository(
            SMSDbContext context,
            ILogger<EstimateRepository> logger) : base(context, logger)
        {
        }
    }
}

