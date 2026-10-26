using Microsoft.AspNetCore.Components.Routing;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for request service operations.
    /// </summary>
    public interface IRoutingGroupService : IGenericService<RoutingGroupDto, RoutingGroup> 
    {
        /// <summary>
        /// Gets routing groups by department ID (from token) and status (active = 1).
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of routing groups filtered by department ID and active status.</returns>
        Task<IEnumerable<RoutingGroupDto>> GetByDepartmentAndStatusAsync(PageParam? pagination = null);

        /// <summary>
        /// Gets all routing groups with employee first names mapped from EmployeeIDs via employee service.
        /// </summary>
        /// <param name="pagination">The pagination parameters (optional).</param>
        /// <returns>A collection of routing group details including EmployeeFirstNames.</returns>
        Task<PagedList<RoutingGroupDetailsDto>> GetAllRoutingGroupDetailsAsync(PageParam? pagination = null);
    }
}

