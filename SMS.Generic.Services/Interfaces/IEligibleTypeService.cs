using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for eligible type service operations.
    /// </summary>
    public interface IEligibleTypeService : IGenericService<EligibleTypeDto, EligibleType>
    {
        /// <summary>
        /// Retrieves an eligible type by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The eligible type DTO if found; otherwise, null.</returns>
        Task<EligibleTypeDto?> GetByIdAsync(long id);
    }
}

