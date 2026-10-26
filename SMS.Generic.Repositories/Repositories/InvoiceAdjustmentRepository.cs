using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Invoice adjustment-specific repository implementation.
    /// </summary>
    public class InvoiceAdjustmentRepository : GenericRepository<InvoiceAdjustment, SMSDbContext>, IInvoiceAdjustmentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceAdjustmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceAdjustmentRepository(
            SMSDbContext context,
            ILogger<InvoiceAdjustmentRepository> logger) : base(context, logger)
        {
        }
    }
}

