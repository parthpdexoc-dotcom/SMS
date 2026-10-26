using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Business type-specific repository implementation.
    /// </summary>
    public class BusinessTypeRepository : GenericRepository<BusinessType, SMSDbContext>, IBusinessTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public BusinessTypeRepository(
            SMSDbContext context,
            ILogger<BusinessTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

