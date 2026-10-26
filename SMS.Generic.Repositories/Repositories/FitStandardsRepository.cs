using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Fit standards-specific repository implementation.
    /// </summary>
    public class FitStandardsRepository : GenericRepository<FitStandards, SMSDbContext>, IFitStandardsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FitStandardsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public FitStandardsRepository(
            SMSDbContext context,
            ILogger<FitStandardsRepository> logger) : base(context, logger)
        {
        }
    }
}

