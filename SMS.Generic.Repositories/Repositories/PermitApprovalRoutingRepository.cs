using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for PermitApprovalRouting entity.
    /// </summary>
    public class PermitApprovalRoutingRepository : GenericRepository<PermitApprovalRouting, SMSDbContext>, IPermitApprovalRoutingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermitApprovalRoutingRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PermitApprovalRoutingRepository(
            SMSDbContext context,
            ILogger<PermitApprovalRoutingRepository> logger)
            : base(context, logger)
        {
        }
    }
}
