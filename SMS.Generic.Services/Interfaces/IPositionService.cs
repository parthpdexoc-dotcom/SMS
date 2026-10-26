using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for position service operations.
    /// </summary>
    public interface IPositionService : IGenericService<PositionDto, Position>
    {
        /// <summary>
        /// Retrieves a position by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The position DTO if found; otherwise, null.</returns>
        Task<PositionDto?> GetByIdAsync(long id);
    }
}

