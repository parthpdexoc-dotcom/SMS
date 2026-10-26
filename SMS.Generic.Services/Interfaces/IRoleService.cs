using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for role service operations.
    /// </summary>
    public interface IRoleService : IGenericService<RoleDto, Role>
    {
        /// <summary>
        /// Retrieves a role by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The role DTO if found; otherwise, null.</returns>
        Task<RoleDto?> GetByIdAsync(int id);
    }
}

