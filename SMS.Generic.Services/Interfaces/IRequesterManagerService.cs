using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester manager service operations.
    /// </summary>
    public interface IRequesterManagerService : IGenericService<RequesterManagerDto, RequesterManager>
    {
        Task<PagedList<RequestManagerResponceDto>> GetAllRequestManagerWithDetailsAsync(PageParam? pageParam, Guid? requesterId = null);
        Task<List<Guid>> GetAssignedRequesterEmployeesByRequesterManagerId(Guid requesterManagerId);
        Task<List<Guid>> GetAssignedRequesterManagerEmployeesByRequesterId(Guid requesterId);

        /// <summary>
        /// Returns the requester-manager row for this requester + manager employee pair, or null if none.
        /// </summary>
        Task<RequesterManagerDto?> GetByRequesterIdAndEmployeeIdAsync(Guid requesterId, Guid employeeId);

        /// <summary>
        /// Requester-manager ids for an employee under a requester.
        /// </summary>
        Task<IReadOnlyList<Guid>> GetRequesterManagerIdsAsync(Guid requesterId, Guid employeeId);

        /// <summary>
        /// Requester-manager ids for an employee across all requesters.
        /// </summary>
        Task<IReadOnlyList<Guid>> GetRequesterManagerIdsByEmployeeIdAsync(Guid employeeId);

        /// <summary>
        /// Tier from requester-manager / requester-employee mappings only (no department-admin check).
        /// </summary>
        Task<AvailablePostReleaseTabTier> GetAvailablePostReleaseTabTierFromMappingsAsync(Guid employeeId, Guid? departmentId);

        /// <summary>
        /// Resolves tier at employee login (no JWT): department-admin via same rule as <c>IsDepartmentAdminPolicy</c> claim, then mappings.
        /// </summary>
        Task<AvailablePostReleaseTabTier> GetAvailablePostReleaseTabTierForLoginAsync(EmployeeLoginInfoDto employeeInfo);

        /// <summary>
        /// Gets lightweight requester manager options for dropdowns (employee name label, searchable).
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null,
            Guid? selectedId = null);

        /// <summary>
        /// Gets employee dropdown options excluding employees already assigned as managers for the requester.
        /// </summary>
        Task<IEnumerable<SelectOptionDto>> GetAvailableEmployeeOptionsAsync(
            string? searchTerm = null,
            Guid? requesterId = null);
    }
}


