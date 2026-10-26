using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for DepartmentQuestionnaire entity operations.
    /// </summary>
    public class DepartmentQuestionnaireRepository : GenericRepository<DepartmentQuestionnaire, SMSDbContext>, IDepartmentQuestionnaireRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentQuestionnaireRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentQuestionnaireRepository(
            SMSDbContext context,
            ILogger<DepartmentQuestionnaireRepository> logger)
            : base(context, logger)
        {
        }
    }
}
