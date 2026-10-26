using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Exempted requester-specific repository implementation.
    /// </summary>
    public class ExemptedRequesterRepository : GenericRepository<ExemptedRequester, SMSDbContext>, IExemptedRequesterRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExemptedRequesterRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public ExemptedRequesterRepository(
            SMSDbContext context,
            ILogger<ExemptedRequesterRepository> logger) : base(context, logger)
        {
        }
    }
}

