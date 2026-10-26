using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for VehicleApprovalRouting service operations.
    /// </summary>
    public interface IVehicleApprovalRoutingService : IGenericService<VehicleApprovalRoutingDto, VehicleApprovalRouting>
    {
        /// <summary>
        /// Upserts multiple vehicle approval routing records with parent-child relationships.
        /// </summary>
        /// <param name="dtos">The collection of DTOs to upsert.</param>
        /// <returns>A collection of upserted DTOs.</returns>
        /// <param name="isSuperUser">When true, audit fields on each DTO are not auto-populated (see <c>IGenericService</c>).</param>
        Task<IEnumerable<VehicleApprovalRoutingDto>> BulkUpsertAsync(IEnumerable<VehicleApprovalRoutingDto> dtos, bool isSuperUser = false);

        /// <summary>
        /// Gets all vehicle approval routing records by department equipment ID with routing group information.
        /// </summary>
        /// <param name="departmentEquipmentId">The department equipment ID to filter by.</param>
        /// <param name="filter">Optional additional filter expression.</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A collection of vehicle approval routing response DTOs with routing group names.</returns>
        Task<IEnumerable<VehicleApprovalRoutingResponseDto>> GetAllVehicleRoutingGroupByDepartmentEquipmentIdAsync(Guid departmentEquipmentId, Expression<Func<VehicleApprovalRouting, bool>>? filter = null, PageParam? pagination = null);
    }
}

