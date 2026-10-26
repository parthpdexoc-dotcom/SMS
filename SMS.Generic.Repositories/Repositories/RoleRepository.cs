using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Role-specific repository implementation.
    /// </summary>
    public class RoleRepository : GenericRepository<Role, SMSDbContext>, IRoleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RoleRepository(
            SMSDbContext context,
            ILogger<RoleRepository> logger) : base(context, logger)
        {
        }
    }
}

