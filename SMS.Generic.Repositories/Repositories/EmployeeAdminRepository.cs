using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee admin-specific repository implementation.
    /// </summary>
    public class EmployeeAdminRepository : GenericRepository<EmployeeAdmin, SMSDbContext>, IEmployeeAdminRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeAdminRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeAdminRepository(
            SMSDbContext context,
            ILogger<EmployeeAdminRepository> logger) : base(context, logger)
        {
        }
    }
}
