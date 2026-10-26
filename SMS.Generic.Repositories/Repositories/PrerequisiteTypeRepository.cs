using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Prerequisite type-specific repository implementation.
    /// </summary>
    public class PrerequisiteTypeRepository : GenericRepository<PrerequisiteType, SMSDbContext>, IPrerequisiteTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrerequisiteTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PrerequisiteTypeRepository(
            SMSDbContext context,
            ILogger<PrerequisiteTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

