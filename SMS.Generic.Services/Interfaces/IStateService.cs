using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for state service operations.
    /// </summary>
    public interface IStateService : IGenericService<StateDto, State>
    {
        /// <summary>
        /// Retrieves a state by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The state DTO if found; otherwise, null.</returns>
        Task<StateDto?> GetByIdAsync(long id);
    }
}

