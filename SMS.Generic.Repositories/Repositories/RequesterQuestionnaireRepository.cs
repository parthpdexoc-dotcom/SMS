using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for PostQuestionnaire entity operations.
    /// </summary>
    public class RequesterQuestionnaireRepository : GenericRepository<RequesterQuestionnaire, SMSDbContext>, IRequesterQuestionnaireRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterQuestionnaireRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterQuestionnaireRepository(
            SMSDbContext context,
            ILogger<RequesterQuestionnaireRepository> logger)
            : base(context, logger)
        {
        }
    }
}
