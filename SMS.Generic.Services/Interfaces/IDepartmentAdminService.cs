using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department admin service operations.
    /// </summary>
    public interface IDepartmentAdminService : IGenericService<DepartmentAdminDto, DepartmentAdmin>
    {
        /// <summary>
        /// Gets distinct employee IDs for department admins matching the given department and admin type.
        /// </summary>
        /// <param name="departmentId">The department identifier.</param>
        /// <param name="adminTypeId">The admin type identifier.</param>
        /// <returns>A collection of employee IDs (IDs only).</returns>
        Task<IEnumerable<Guid>> GetEmployeeIdsByDepartmentAndAdminTypeAsync(Guid departmentId, Guid adminTypeId);
    }
}
