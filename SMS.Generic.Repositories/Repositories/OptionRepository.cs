using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for Option entity operations.
    /// </summary>
    public class OptionRepository : GenericRepository<Option, SMSDbContext>, IOptionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public OptionRepository(
            SMSDbContext context,
            ILogger<OptionRepository> logger)
            : base(context, logger)
        {
        }
    }
}
