using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service interface for DepartmentEquipment.
    /// </summary>
    public interface IDepartmentDefinedFieldsService : IGenericService<DepartmentDefinedFieldsDto, DepartmentDefinedFields>
    {
        /// <summary>
        /// Ensures a department defined fields record exists for the given department; creates one if none exists.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        Task EnsureExistsForDepartmentAsync(Guid departmentId);

        /// <summary>
        /// Gets the department defined fields for the given department.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        Task<DepartmentDefinedFieldsDto?> GetByDepartmentIdAsync(Guid departmentId);

        /// <summary>
        /// Deletes all department defined fields records for the given department.
        /// </summary>
        /// <param name="departmentId">The department identifier (Guid).</param>
        /// <returns>True if any records were deleted; otherwise, false.</returns>
        Task<bool> DeleteByDepartmentIdAsync(Guid departmentId);
    }
}


