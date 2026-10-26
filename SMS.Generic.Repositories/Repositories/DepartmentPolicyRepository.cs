using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department policy-specific repository implementation.
    /// </summary>
    public class DepartmentPolicyRepository : GenericRepository<DepartmentPolicy, SMSDbContext>, IDepartmentPolicyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentPolicyRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentPolicyRepository(
            SMSDbContext context,
            ILogger<DepartmentPolicyRepository> logger) : base(context, logger)
        {
        }
    }
}
