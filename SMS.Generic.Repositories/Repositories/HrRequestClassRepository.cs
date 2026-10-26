using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// HR request class-specific repository implementation.
    /// </summary>
    public class HrRequestClassRepository : GenericRepository<HrRequestClass, SMSDbContext>, IHrRequestClassRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HrRequestClassRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public HrRequestClassRepository(
            SMSDbContext context,
            ILogger<HrRequestClassRepository> logger) : base(context, logger)
        {
        }
    }
}

