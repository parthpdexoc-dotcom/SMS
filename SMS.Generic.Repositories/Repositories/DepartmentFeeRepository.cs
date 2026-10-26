using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for department fee operations.
    /// </summary>
    public class DepartmentFeeRepository : GenericRepository<DepartmentFee, SMSDbContext>, IDepartmentFeeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentFeeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentFeeRepository(
            SMSDbContext context,
            ILogger<GenericRepository<DepartmentFee, SMSDbContext>> logger)
            : base(context, logger)
        { }
    }
}
