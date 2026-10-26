using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// State-specific repository implementation.
    /// </summary>
    public class StateRepository : GenericRepository<State, SMSDbContext>, IStateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StateRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public StateRepository(
            SMSDbContext context,
            ILogger<StateRepository> logger) : base(context, logger)
        {
        }
    }
}

