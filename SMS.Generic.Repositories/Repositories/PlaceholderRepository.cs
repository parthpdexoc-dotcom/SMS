using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class PlaceholderRepository : GenericRepository<Placeholder, SMSDbContext>, IPlaceholderRepository
    {
        public PlaceholderRepository(SMSDbContext context, ILogger<PlaceholderRepository> logger)
            : base(context, logger)
        {
        }
    }
}
