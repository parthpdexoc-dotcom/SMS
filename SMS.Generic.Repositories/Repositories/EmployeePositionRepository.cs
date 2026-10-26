using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee position-specific repository implementation.
    /// </summary>
    public class EmployeePositionRepository : GenericRepository<EmployeePosition, SMSDbContext>, IEmployeePositionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeePositionRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeePositionRepository(
            SMSDbContext context,
            ILogger<EmployeePositionRepository> logger) : base(context, logger)
        {
        }
    }
}

