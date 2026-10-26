using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department divisions service operations.
    /// </summary>
    public interface IDepartmentDivisionsService : IGenericService<DepartmentDivisionsDto, DepartmentDivisions>
    {
        /// <summary>
        /// Gets a department division by department and name (case-insensitive match on DivisionShort or DivisionLong).
        /// </summary>
        Task<DepartmentDivisionsDto?> GetByDepartmentIdAndNameAsync(Guid departmentId, string name);

        /// <summary>
        /// Ensures that a default department division exists for the given department.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        Task EnsureDefaultDepartmentDivisionExistsAsync(Guid departmentId);
    }
}
