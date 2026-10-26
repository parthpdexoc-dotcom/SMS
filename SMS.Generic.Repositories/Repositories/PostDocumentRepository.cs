using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Generic.Repositories.Repositories
{
   
    public class PostDocumentRepository : GenericRepository<PostDocument, SMSDbContext>, IPostDocumentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostDocumentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public PostDocumentRepository(
            SMSDbContext context,
            ILogger<PostDocumentRepository> logger) : base(context, logger)
        {
        }
        }
}
