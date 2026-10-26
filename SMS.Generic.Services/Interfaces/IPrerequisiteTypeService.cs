using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for prerequisite type service operations.
    /// </summary>
    public interface IPrerequisiteTypeService : IGenericService<PrerequisiteTypeDto, PrerequisiteType>
    {
        /// <summary>
        /// Retrieves a prerequisite type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The prerequisite type DTO if found; otherwise, null.</returns>
        Task<PrerequisiteTypeDto?> GetByIdAsync(int id);
    }
}

