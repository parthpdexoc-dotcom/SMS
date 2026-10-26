using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Prerequisite-specific repository implementation.
    /// </summary>
    public class PrerequisiteRepository : GenericRepository<Prerequisite, SMSDbContext>, IPrerequisiteRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PrerequisiteRepository(
            SMSDbContext context,
            ILogger<PrerequisiteRepository> logger) : base(context, logger)
        {
        }
    }
}

