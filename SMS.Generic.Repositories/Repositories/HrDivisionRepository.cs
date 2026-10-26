using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// HR division-specific repository implementation.
    /// </summary>
    public class HrDivisionRepository : GenericRepository<HrDivision, SMSDbContext>, IHrDivisionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HrDivisionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public HrDivisionRepository(
            SMSDbContext context,
            ILogger<HrDivisionRepository> logger) : base(context, logger)
        {
        }
    }
}

