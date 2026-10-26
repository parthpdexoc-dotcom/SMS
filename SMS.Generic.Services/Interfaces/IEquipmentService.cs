using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for equipment service operations.
    /// </summary>
    public interface IEquipmentService : IGenericService<EquipmentDto, Equipment>
    {
        /// <summary>
        /// Retrieves an equipment by its primary key (EquipementID as Guid).
        /// </summary>
        /// <param name="id">The EquipementID value.</param>
        /// <returns>The equipment DTO if found; otherwise, null.</returns>
        Task<EquipmentDto?> GetByIdAsync(Guid id);
    }
}

