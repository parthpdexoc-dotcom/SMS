using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Close request details-specific repository implementation.
    /// </summary>
    public class CloseRequestDetailsRepository : GenericRepository<CloseRequestDetails, SMSDbContext>, ICloseRequestDetailsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloseRequestDetailsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public CloseRequestDetailsRepository(
            SMSDbContext context,
            ILogger<CloseRequestDetailsRepository> logger) : base(context, logger)
        {
        }
    }
}

