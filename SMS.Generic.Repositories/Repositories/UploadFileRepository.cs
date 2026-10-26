using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Upload file repository implementation.
    /// </summary>
    public class UploadFileRepository : GenericRepository<UploadFile, SMSDbContext>, IUploadFileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public UploadFileRepository(
            SMSDbContext context,
            ILogger<UploadFileRepository> logger)
            : base(context, logger)
        {
        }
    }
}
