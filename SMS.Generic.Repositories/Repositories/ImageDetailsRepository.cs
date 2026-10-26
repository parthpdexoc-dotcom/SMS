using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Image details-specific repository implementation.
    /// </summary>
    public class ImageDetailsRepository : GenericRepository<ImageDetails, SMSDbContext>, IImageDetailsRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageDetailsRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ImageDetailsRepository(
            SMSDbContext context,
            ILogger<ImageDetailsRepository> logger) : base(context, logger)
        {
        }
    }
}

