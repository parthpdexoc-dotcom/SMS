using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for EmployeeDeviceToken operations.
    /// </summary>
    public class EmployeeDeviceTokenRepository : GenericRepository<EmployeeDeviceToken, SMSDbContext>, IEmployeeDeviceTokenRepository
    {
        public EmployeeDeviceTokenRepository(
            SMSDbContext context,
            ILogger<EmployeeDeviceTokenRepository> logger)
            : base(context, logger)
        { }
    }
}
