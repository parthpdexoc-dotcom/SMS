using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Repository for Response entity operations.
    /// </summary>
    public class ResponseRepository : GenericRepository<Response, SMSDbContext>, IResponseRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ResponseRepository(
            SMSDbContext context,
            ILogger<ResponseRepository> logger)
            : base(context, logger)
        {
        }
    }
}
