using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for estimate service operations.
    /// </summary>
    public interface IEstimateService : IGenericService<EstimateDto, Estimate>
    {
        /// <summary>
        /// Retrieves an estimate by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The estimate DTO if found; otherwise, null.</returns>
        Task<EstimateDto?> GetByIdAsync(long id);
    }
}

