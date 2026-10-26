using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository implementation for EmployeeTenure.
    /// </summary>
    public class EmployeeTenureRepository : GenericRepository<EmployeeTenure, SMSDbContext>, IEmployeeTenureRepository
    {
        public EmployeeTenureRepository(SMSDbContext context, ILogger<EmployeeTenureRepository> logger)
            : base(context, logger)
        {
        }
    }
}


