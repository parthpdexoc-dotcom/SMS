using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS.Data;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Repositories.Interfaces;

namespace SMS.Generic.Repositories.Repositories
{
    /// <summary>
    /// Invoice equipment list-specific repository implementation.
    /// </summary>
    public class InvoiceEquipmentListRepository : GenericRepository<InvoiceEquipmentList, SMSDbContext>, IInvoiceEquipmentListRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceEquipmentListRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        public InvoiceEquipmentListRepository(
            SMSDbContext context,
            ILogger<InvoiceEquipmentListRepository> logger) : base(context, logger)
        {
        }
    }
}

