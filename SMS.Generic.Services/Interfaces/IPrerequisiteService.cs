using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for prerequisite service operations.
    /// </summary>
    public interface IPrerequisiteService : IGenericService<PrerequisiteDto, Prerequisite>
    {
        /// <summary>
        /// Retrieves a prerequisite by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The prerequisite DTO if found; otherwise, null.</returns>
        Task<PrerequisiteDto?> GetByIdAsync(int id);
    }
}

