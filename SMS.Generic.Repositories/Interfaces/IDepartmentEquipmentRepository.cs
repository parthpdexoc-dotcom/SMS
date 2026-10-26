using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for DepartmentEquipment.
    /// </summary>
    public interface IDepartmentEquipmentRepository : IGenericRepository<DepartmentEquipment>
    {
        Task<List<string>> GetVehicleNamesByDepartmentEquipmentIdsAsync(IEnumerable<Guid> departmentEquipmentIds);
    }
}


