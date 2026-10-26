using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department-specific repository implementation.
    /// </summary>
    public class DepartmentRepository : GenericRepository<Department, SMSDbContext>, IDepartmentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentRepository(
            SMSDbContext context,
            ILogger<DepartmentRepository> logger) : base(context, logger)
        {
        }
    }
}

