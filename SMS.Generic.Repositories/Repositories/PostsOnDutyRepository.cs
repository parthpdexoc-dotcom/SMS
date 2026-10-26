using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Posts on duty-specific repository implementation.
    /// </summary>
    public class PostsOnDutyRepository : GenericRepository<PostsOnDuty, SMSDbContext>, IPostsOnDutyRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostsOnDutyRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PostsOnDutyRepository(
            SMSDbContext context,
            ILogger<PostsOnDutyRepository> logger) : base(context, logger)
        {
        }
    }
}

