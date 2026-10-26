using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class RoutingGroupRepository  : GenericRepository<RoutingGroup, SMSDbContext>, IRoutingGroupRepository
    {
        public RoutingGroupRepository(
        SMSDbContext context,
        ILogger<RoutingGroupRepository> logger)
        : base(context, logger)
        { }
    }
}
