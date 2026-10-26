using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Department invoice config-specific repository implementation.
    /// </summary>
    public class DepartmentInvoiceConfigRepository : GenericRepository<DepartmentInvoiceConfig, SMSDbContext>, IDepartmentInvoiceConfigRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentInvoiceConfigRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public DepartmentInvoiceConfigRepository(
            SMSDbContext context,
            ILogger<DepartmentInvoiceConfigRepository> logger) : base(context, logger)
        {
        }
    }
}
