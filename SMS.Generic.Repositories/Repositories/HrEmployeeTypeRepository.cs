using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// HR employee type-specific repository implementation.
    /// </summary>
    public class HrEmployeeTypeRepository : GenericRepository<HrEmployeeType, SMSDbContext>, IHrEmployeeTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HrEmployeeTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public HrEmployeeTypeRepository(
            SMSDbContext context,
            ILogger<HrEmployeeTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

