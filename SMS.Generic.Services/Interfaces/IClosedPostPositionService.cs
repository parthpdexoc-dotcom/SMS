using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for closed post position service operations.
    /// </summary>
    public interface IClosedPostPositionService : IGenericService<ClosedPostPositionDto, ClosedPostPosition>
    {
        /// <summary>
        /// Updates closed post position status and optionally IsEdited flag.
        /// </summary>
        /// <param name="closedPostPositionId">Closed post position id.</param>
        /// <param name="status">Target closed post status.</param>
        /// <param name="isEdited">Optional IsEdited value to set.</param>
        /// <returns>Number of updated records.</returns>
        Task<int> UpdateClosedPostPositionStatusAsync(Guid closedPostPositionId, ClosedPostPositionStatus status, bool? isEdited = null);

        /// <summary>
        /// Gets the latest closed post position per distinct post position id for an employee, paged (in-memory after load).
        /// When <paramref name="employeeId"/> is null, uses the employee from the auth token (controller enforces privileged override).
        /// </summary>
        /// <param name="pagination">Pagination (offset/limit or page/pageSize).</param>
        /// <param name="employeeId">Target employee id when resolving as another user via admin-only query param.</param>
        Task<PagedList<EmployeePostListingItemDto>> GetLatestPerPostPositionPagedAsync(PageParam? pagination = null, Guid? employeeId = null);

        /// <summary>
        /// Paged list from the closed post position table. Filters on <see cref="ClosedPostPositionStatus"/> (default: Closed and Unclosed),
        /// with the same search, department, date range, and optional employee id filters as the post position close-assignments list.
        /// When <paramref name="isPlaceholderOnly"/> is true, returns only placeholder post positions.
        /// </summary>
        Task<PagedList<CloseAssignmentClosedPostPositionResponseDto>> GetCloseAssignmentsForLoggedInEmployeeAsync(
            string? searchTerm = null,
            PageParam? pagination = null,
            Guid? employeeId = null,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            bool isPlaceholderOnly = false,
            Guid? requesterId = null);
    }
}
