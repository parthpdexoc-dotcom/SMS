using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for Questionnaire entity operations.
    /// </summary>
    public class QuestionnaireRepository : GenericRepository<Questionnaire, SMSDbContext>, IQuestionnaireRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public QuestionnaireRepository(
            SMSDbContext context,
            ILogger<QuestionnaireRepository> logger)
            : base(context, logger)
        {
        }
    }
}
