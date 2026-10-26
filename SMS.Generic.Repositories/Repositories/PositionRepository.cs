using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Position-specific repository implementation.
    /// </summary>
    public class PositionRepository : GenericRepository<Position, SMSDbContext>, IPositionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PositionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PositionRepository(
            SMSDbContext context,
            ILogger<PositionRepository> logger) : base(context, logger)
        {
        }
    }
}

