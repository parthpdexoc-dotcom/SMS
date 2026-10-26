using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee status-specific repository implementation.
    /// </summary>
    public class EmployeeStatusRepository : GenericRepository<EmployeeStatus, SMSDbContext>, IEmployeeStatusRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeStatusRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeStatusRepository(
            SMSDbContext context,
            ILogger<EmployeeStatusRepository> logger) : base(context, logger)
        {
        }
    }
}

