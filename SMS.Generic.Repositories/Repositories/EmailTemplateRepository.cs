using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Email template-specific repository implementation.
    /// </summary>
    public class EmailTemplateRepository : GenericRepository<EmailTemplate, SMSDbContext>, IEmailTemplateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailTemplateRepository(
            SMSDbContext context,
            ILogger<EmailTemplateRepository> logger) : base(context, logger)
        {
        }

        /// <inheritdoc />
        public async Task<EmailTemplate?> GetByEmailTypeAsync(EmailType emailType)
        {
            var results = await GetAllAsync(e => e.EmailType == emailType, null, new Generic.Common.Implementations.Paginations.PageParam { Offset = 0, Limit = 1 });
            return results.FirstOrDefault();
        }
    }
}
