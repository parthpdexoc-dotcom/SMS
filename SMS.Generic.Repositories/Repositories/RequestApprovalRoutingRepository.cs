using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for RequestApprovalRouting entity.
    /// </summary>
    public class RequestApprovalRoutingRepository : GenericRepository<RequestApprovalRouting, SMSDbContext>, IRequestApprovalRoutingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestApprovalRoutingRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public RequestApprovalRoutingRepository(
            SMSDbContext context,
            ILogger<RequestApprovalRoutingRepository> logger)
            : base(context, logger)
        {
        }
    }
}

