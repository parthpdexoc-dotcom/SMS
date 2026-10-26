using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Post type-specific repository implementation.
    /// </summary>
    public class PostTypeRepository : GenericRepository<PostType, SMSDbContext>, IPostTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostTypeRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PostTypeRepository(
            SMSDbContext context,
            ILogger<PostTypeRepository> logger) : base(context, logger)
        {
        }
    }
}

