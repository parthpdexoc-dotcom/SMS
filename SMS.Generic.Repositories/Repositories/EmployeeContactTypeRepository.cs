using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Employee contact type-specific repository implementation.
    /// </summary>
    public class EmployeeContactTypeRepository : GenericRepository<EmployeeContactType, SMSDbContext>, IEmployeeContactTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeContactTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmployeeContactTypeRepository(
            SMSDbContext context,
            ILogger<EmployeeContactTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

