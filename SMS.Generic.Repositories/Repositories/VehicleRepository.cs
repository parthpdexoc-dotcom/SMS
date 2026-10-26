using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Vehicle-specific repository implementation.
    /// </summary>
    public class VehicleRepository : GenericRepository<Vehicle, SMSDbContext>, IVehicleRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public VehicleRepository(
            SMSDbContext context,
            ILogger<VehicleRepository> logger) : base(context, logger)
        {
        }
    }
}

