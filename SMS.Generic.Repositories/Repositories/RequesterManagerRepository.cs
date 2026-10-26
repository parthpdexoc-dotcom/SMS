using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// RequesterManager-specific repository implementation.
    /// </summary>
    public class RequesterManagerRepository : GenericRepository<RequesterManager, SMSDbContext>, IRequesterManagerRepository
    {
        public RequesterManagerRepository(SMSDbContext context, ILogger<RequesterManagerRepository> logger)
            : base(context, logger)
        {
        }
    }
}


