using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department configuration-specific repository implementation.
    /// </summary>
    public class DepartmentConfigurationRepository : GenericRepository<DepartmentConfiguration, SMSDbContext>, IDepartmentConfigurationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentConfigurationRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentConfigurationRepository(
            SMSDbContext context,
            ILogger<DepartmentConfigurationRepository> logger) : base(context, logger)
        {
        }
    }
}

