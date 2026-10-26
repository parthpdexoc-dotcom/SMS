using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for Answers entity operations.
    /// </summary>
    public class AnswersRepository : GenericRepository<Answers, SMSDbContext>, IAnswersRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswersRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public AnswersRepository(
            SMSDbContext context,
            ILogger<AnswersRepository> logger)
            : base(context, logger)
        {
        }
    }
}
