using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department admin-specific repository implementation.
    /// </summary>
    public class DepartmentAdminRepository : GenericRepository<DepartmentAdmin, SMSDbContext>, IDepartmentAdminRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentAdminRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentAdminRepository(
            SMSDbContext context,
            ILogger<DepartmentAdminRepository> logger) : base(context, logger)
        {
        }
    }
}
