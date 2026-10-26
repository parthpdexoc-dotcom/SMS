using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for invoice adjustment service operations.
    /// </summary>
    public interface IInvoiceAdjustmentService : IGenericService<InvoiceAdjustmentDto, InvoiceAdjustment>
    {
        /// <summary>
        /// Retrieves an invoice adjustment by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice adjustment DTO if found; otherwise, null.</returns>
        Task<InvoiceAdjustmentDto?> GetByIdAsync(long id);
    }
}

