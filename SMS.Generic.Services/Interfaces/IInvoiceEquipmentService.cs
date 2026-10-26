using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for invoice equipment service operations.
    /// </summary>
    public interface IInvoiceEquipmentService : IGenericService<InvoiceEquipmentDto, InvoiceEquipment>
    {
        /// <summary>
        /// Retrieves an invoice equipment by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice equipment DTO if found; otherwise, null.</returns>
        Task<InvoiceEquipmentDto?> GetByIdAsync(int id);
    }
}

