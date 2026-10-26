using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department configuration service operations.
    /// </summary>
    public interface IDepartmentConfigurationService : IGenericService<DepartmentConfigurationDto, DepartmentConfiguration>
    {
        /// <summary>
        /// Gets a department configuration by DepartmentID.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        /// <returns>The department configuration if found; otherwise, null.</returns>
        Task<DepartmentConfigurationDto?> GetByDepartmentIdAsync(Guid departmentId);

        /// <summary>
        /// Ensures a department configuration record exists for the given department; creates one if none exists.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        Task EnsureExistsForDepartmentAsync(Guid departmentId);

        /// <summary>
        /// Deletes all department configuration records for the given department.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        /// <returns>True if any records were deleted; otherwise, false.</returns>
        Task<bool> DeleteByDepartmentIdAsync(Guid departmentId);
    }
}

