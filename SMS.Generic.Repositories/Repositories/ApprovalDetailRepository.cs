using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for ApprovalDetail entity (ApprovalDetails table).
    /// </summary>
    public class ApprovalDetailRepository : GenericRepository<ApprovalDetail, SMSDbContext>, IApprovalDetailRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalDetailRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ApprovalDetailRepository(
            SMSDbContext context,
            ILogger<GenericRepository<ApprovalDetail, SMSDbContext>> logger)
            : base(context, logger)
        {
        }
    }
}
