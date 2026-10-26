using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for block date operations.
    /// </summary>
    public class BlockDateRepository : GenericRepository<BlockDate, SMSDbContext>, IBlockDateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDateRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public BlockDateRepository(
            SMSDbContext context,
            ILogger<GenericRepository<BlockDate, SMSDbContext>> logger)
            : base(context, logger)
        { }
    }
}
