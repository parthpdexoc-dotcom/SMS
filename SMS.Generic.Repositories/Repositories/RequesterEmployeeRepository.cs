using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for RequesterEmployee operations.
    /// </summary>
    public class RequesterEmployeeRepository : GenericRepository<RequesterEmployee, SMSDbContext>, IRequesterEmployeeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterEmployeeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        public RequesterEmployeeRepository(
            SMSDbContext context,
            ILogger<RequesterEmployeeRepository> logger)
            : base(context, logger)
        { }
    }
}
