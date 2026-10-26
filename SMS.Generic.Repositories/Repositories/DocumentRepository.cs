using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Document-specific repository implementation.
    /// </summary>
    public class DocumentRepository : GenericRepository<Document, SMSDbContext>, IDocumentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DocumentRepository(
            SMSDbContext context,
            ILogger<DocumentRepository> logger) : base(context, logger)
        {
        }
    }
}
