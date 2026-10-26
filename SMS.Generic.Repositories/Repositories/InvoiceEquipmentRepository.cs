using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Invoice equipment-specific repository implementation.
    /// </summary>
    public class InvoiceEquipmentRepository : GenericRepository<InvoiceEquipment, SMSDbContext>, IInvoiceEquipmentRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceEquipmentRepository(
            SMSDbContext context,
            ILogger<InvoiceEquipmentRepository> logger) : base(context, logger)
        {
        }
    }
}

