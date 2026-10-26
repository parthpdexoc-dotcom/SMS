using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// EmployeeSkill-specific repository implementation.
    /// </summary>
    public class EmployeeSkillRepository : GenericRepository<EmployeeSkill, SMSDbContext>, IEmployeeSkillRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeSkillRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeSkillRepository(
            SMSDbContext context,
            ILogger<EmployeeSkillRepository> logger) : base(context, logger)
        {
        }
    }
}

