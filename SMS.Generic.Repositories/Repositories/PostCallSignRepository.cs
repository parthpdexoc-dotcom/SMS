using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    public class PostCallSignRepository : GenericRepository<PostCallSign, SMSDbContext>, IPostCallSignRepository
    {
        public PostCallSignRepository(
            SMSDbContext context,
            ILogger<PostCallSignRepository> logger) : base(context, logger)
        {
        }
    }
}
