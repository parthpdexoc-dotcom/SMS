using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for fit standards service operations.
    /// </summary>
    public interface IFitStandardsService : IGenericService<FitStandardsDto, FitStandards>
    {
        /// <summary>
        /// Retrieves a fit standards record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The fit standards DTO if found; otherwise, null.</returns>
        Task<FitStandardsDto?> GetByIdAsync(int id);
    }
}

