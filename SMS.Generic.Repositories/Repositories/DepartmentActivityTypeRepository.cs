using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department activity type-specific repository implementation.
    /// </summary>
    public class DepartmentActivityTypeRepository : GenericRepository<DepartmentActivityType, SMSDbContext>, IDepartmentActivityTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentActivityTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentActivityTypeRepository(
            SMSDbContext context,
            ILogger<DepartmentActivityTypeRepository> logger) : base(context, logger)
        {
        }
    }
}
