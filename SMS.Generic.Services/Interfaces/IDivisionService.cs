using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for division service operations.
    /// </summary>
    public interface IDivisionService : IGenericService<DivisionDto, Division>
    {
        /// <summary>
        /// Retrieves a division by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The division DTO if found; otherwise, null.</returns>
        Task<DivisionDto?> GetByIdAsync(int id);
    }
}



