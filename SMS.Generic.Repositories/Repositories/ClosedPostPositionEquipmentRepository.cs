using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for ClosedPostPositionEquipment entity operations.
    /// </summary>
    public class ClosedPostPositionEquipmentRepository : GenericRepository<ClosedPostPositionEquipment, SMSDbContext>, IClosedPostPositionEquipmentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionEquipmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ClosedPostPositionEquipmentRepository(
            SMSDbContext context,
            ILogger<ClosedPostPositionEquipmentRepository> logger)
            : base(context, logger)
        {
        }
    }
}
