using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for question operations.
    /// </summary>
    public class QuestionRepository : GenericRepository<Question, SMSDbContext>, IQuestionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public QuestionRepository(
            SMSDbContext context,
            ILogger<QuestionRepository> logger)
            : base(context, logger)
        {
        }
    }
}
