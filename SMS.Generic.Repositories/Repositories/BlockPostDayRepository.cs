using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Block post day-specific repository implementation.
    /// </summary>
    public class BlockPostDayRepository : GenericRepository<BlockPostDay, SMSDbContext>, IBlockPostDayRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockPostDayRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public BlockPostDayRepository(
            SMSDbContext context,
            ILogger<BlockPostDayRepository> logger) : base(context, logger)
        {
        }
    }
}

