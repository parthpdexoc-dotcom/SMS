using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// RequesterAuthorized-specific repository implementation.
    /// </summary>
    public class RequesterAuthorizedRepository : GenericRepository<RequesterAuthorized, SMSDbContext>, IRequesterAuthorizedRepository
    {
        public RequesterAuthorizedRepository(SMSDbContext context, ILogger<RequesterAuthorizedRepository> logger)
            : base(context, logger)
        {
        }
    }
}


