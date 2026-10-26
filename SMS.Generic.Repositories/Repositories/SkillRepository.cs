using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Skill-specific repository implementation.
    /// </summary>
    public class SkillRepository : GenericRepository<Skill, SMSDbContext>, ISkillRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkillRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public SkillRepository(
            SMSDbContext context,
            ILogger<SkillRepository> logger) : base(context, logger)
        {
        }
    }
}
