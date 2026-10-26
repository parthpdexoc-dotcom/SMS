using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for invoice equipment list service operations.
    /// </summary>
    public interface IInvoiceEquipmentListService : IGenericService<InvoiceEquipmentListDto, InvoiceEquipmentList>
    {
        /// <summary>
        /// Retrieves an invoice equipment list by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The invoice equipment list DTO if found; otherwise, null.</returns>
        Task<InvoiceEquipmentListDto?> GetByIdAsync(int id);
    }
}

