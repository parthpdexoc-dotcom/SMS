using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Division-specific repository implementation.
    /// </summary>
    public class DivisionRepository : GenericRepository<Division, SMSDbContext>, IDivisionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DivisionRepository(
            SMSDbContext context,
            ILogger<DivisionRepository> logger) : base(context, logger)
        {
        }
    }
}



