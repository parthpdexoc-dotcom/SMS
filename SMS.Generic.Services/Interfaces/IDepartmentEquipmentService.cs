using System;
using System.Collections.Generic;
using SMS.Generic.Common.Enums;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service interface for DepartmentEquipment.
    /// </summary>
    public interface IDepartmentEquipmentService : IGenericService<DepartmentEquipmentDto, DepartmentEquipment>
    {
        Task<IEnumerable<DepartmentEquipmentResponseDto>> GetDepartmentEquipmentByTypeAsync(Guid departmentId, EquipmentCategory category, IEnumerable<Guid>? selectedIds = null);

        Task<List<string>> GetVehicleNamesByDepartmentEquipmentIdsAsync(IEnumerable<Guid> ids);

        /// <summary>
        /// Loads display names keyed by department equipment id (equipment type short, else long).
        /// </summary>
        Task<IReadOnlyDictionary<Guid, string?>> GetEquipmentDisplayNamesByIdsAsync(IEnumerable<Guid> departmentEquipmentIds);

        /// <summary>
        /// Lightweight dropdown options for active department vehicles.
        /// </summary>
        Task<IEnumerable<DepartmentEquipmentSelectOptionDto>> GetDepartmentVehicleOptionsAsync(
            IEnumerable<Guid>? selectedIds = null,
            string? searchTerm = null);

        /// <summary>
        /// Lightweight dropdown options for active department equipment (non-vehicle).
        /// </summary>
        Task<IEnumerable<DepartmentEquipmentSelectOptionDto>> GetDepartmentEquipmentOptionsAsync(
            IEnumerable<Guid>? selectedIds = null,
            string? searchTerm = null);
    }
}

