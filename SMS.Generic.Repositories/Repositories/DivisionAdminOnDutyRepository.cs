using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Division admin on duty-specific repository implementation.
    /// </summary>
    public class DivisionAdminOnDutyRepository : GenericRepository<DivisionAdminOnDuty, SMSDbContext>, IDivisionAdminOnDutyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionAdminOnDutyRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DivisionAdminOnDutyRepository(
            SMSDbContext context,
            ILogger<DivisionAdminOnDutyRepository> logger) : base(context, logger)
        {
        }
    }
}

