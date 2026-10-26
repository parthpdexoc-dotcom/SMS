using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department policy service operations.
    /// </summary>
    public interface IDepartmentPolicyService : IGenericService<DepartmentPolicyDto, DepartmentPolicy>
    {
        Task<DepartmentPolicyDto?> GetDepartmentPolicyConfigurationByDepartmentId(Guid departmentId);

        Task EnsureExistsForDepartmentAsync(Guid departmentId);
    }
}
