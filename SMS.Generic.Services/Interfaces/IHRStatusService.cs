using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for HR status service operations.
    /// </summary>
    public interface IHRStatusService : IGenericService<HRStatusDto, HRStatus>
    {
        /// <summary>
        /// Retrieves an HR status by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The HR status DTO if found; otherwise, null.</returns>
        Task<HRStatusDto?> GetByIdAsync(int id);
    }
}

