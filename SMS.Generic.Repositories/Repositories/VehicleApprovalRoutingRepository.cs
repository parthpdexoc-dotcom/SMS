using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for VehicleApprovalRouting entity.
    /// </summary>
    public class VehicleApprovalRoutingRepository : GenericRepository<VehicleApprovalRouting, SMSDbContext>, IVehicleApprovalRoutingRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApprovalRoutingRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public VehicleApprovalRoutingRepository(
            SMSDbContext context,
            ILogger<VehicleApprovalRoutingRepository> logger)
            : base(context, logger)
        {
        }
    }
}

