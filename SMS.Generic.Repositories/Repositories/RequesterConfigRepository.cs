using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// RequesterConfig-specific repository implementation.
    /// </summary>
    public class RequesterConfigRepository : GenericRepository<RequesterConfig, SMSDbContext>, IRequesterConfigRepository
    {
        private readonly SMSDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterConfigRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequesterConfigRepository(
            SMSDbContext context,
            ILogger<RequesterConfigRepository> logger) : base(context, logger)
        {
            _context = context;
        }
    }
}
