using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for division admin on duty service operations.
    /// </summary>
    public interface IDivisionAdminOnDutyService : IGenericService<DivisionAdminOnDutyDto, DivisionAdminOnDuty>
    {
        /// <summary>
        /// Retrieves a division admin on duty by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The division admin on duty DTO if found; otherwise, null.</returns>
        Task<DivisionAdminOnDutyDto?> GetByIdAsync(int id);
    }
}

