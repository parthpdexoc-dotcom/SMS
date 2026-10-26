using SMS.Generic.Common.Enums;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Common.Implementations.Responses;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for tenure waiting position service operations.
    /// </summary>
    public interface IWaitingPostPositionService : IGenericService<WaitingPostPositionDto, WaitingPostPosition>
    {
        /// <summary>
        /// Gets tenure waiting positions for the currently logged-in employee only,
        /// filtered to rows whose <see cref="WaitingPostPositionDto.ReleaseAs"/> is in <paramref name="releaseAsValues"/>.
        /// </summary>
        /// <param name="releaseAsValues">One or more release-as values to include (required).</param>
        /// <param name="isViewWLHistory">When true, includes all rows; when false, only rows with shift start within the last 7 days through future (department timezone).</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A paged list of tenure waiting positions for the logged-in employee.</returns>
        Task<PagedList<EmployeePostListingItemDto>> GetForLoggedInEmployeeAsync(
            IReadOnlyList<ReleaseAs> releaseAsValues,
            bool isViewWLHistory = false,
            PageParam? pagination = null);

        Task<SuccessResult<WaitingPostPositionDto>> AssignWaitingPostPositiontoMeAsync(Guid postId, DateOnly dateOfWork, Guid? employeeId = null);

        /// <summary>
        /// Creates a waiting post position row for a special-event request (no post id yet), status <see cref="PostPositionStatus.SpecialEvent"/>.
        /// Uses <paramref name="employeeId"/> when provided; otherwise the current token employee id.
        /// </summary>
        Task<SuccessResult<WaitingPostPositionDto>> ApplySpecialEventAsync(Guid requestId, DateOnly dateOfWork, Guid? employeeId = null);

        /// <summary>
        /// Updates the status of a waiting post position owned by the logged-in employee.
        /// </summary>
        /// <param name="waitingPostPositionId">The waiting post position ID.</param>
        /// <param name="status">The target status.</param>
        /// <returns>The number of updated rows.</returns>
        Task<int> UpdateStatusByIdAsync(Guid waitingPostPositionId, PostPositionStatus status);

        /// <summary>
        /// Read-only: returns tenure waiting list rows for the given post IDs (for background jobs/testing).
        /// Includes Post and its Request.
        /// </summary>
        Task<List<WaitingPostPositionDto>> GetForPostsForJobAsync(List<Guid> postIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Job helper: deletes specific tenure waiting rows by id.
        /// Only deletes the provided ids (does not delete all).
        /// </summary>
        Task<bool> BulkDeleteForJobAsync(List<Guid> tenureWaitingPositionIds, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a waiting-list row as assigned (status Applied, order number 0) and decrements order for remaining Waiting rows on the same post and date.
        /// </summary>
        Task<bool> MarkWaitingRowAssignedAsync(Guid waitingPostPositionId, Guid? assignedPostId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Job-only: sets a tenure waiting row to <see cref="PostPositionStatus.Declined"/> and clears its queue slot (<c>OrderNumber</c> 0).
        /// Remaining <see cref="PostPositionStatus.Waiting"/> rows are renumbered by <see cref="ReorderWaitingPositionsByGivenOrderNumbersAsync"/> after processing (no sibling order updates here).
        /// </summary>
        Task<bool> MarkWaitingRowDeclinedForJobAsync(Guid waitingPostPositionId, CancellationToken cancellationToken = default);
        Task<bool> UpdateProcessingStatusAsync(Guid id, ProcessingStatus status, string? message = null, int? statusCode = null, bool markProcessed = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Active waiting-list rows (status Waiting) for a post and work date, including employee details.
        /// </summary>
        Task<List<WaitingPostPositionResponseDto>> GetWaitingListEmployee(Guid postId, DateOnly dateOfWork);
        Task<List<WaitingPostPositionResponseDto>> GetSpecialEventEmployeeIdsAsync(Guid requestId, DateOnly dateOfWork);
        Task<Dictionary<Guid, WaitingPostPositionResponseDto>> GetPendingSpecialEventRowsByEmployeeAsync(Guid requestId, DateOnly dateOfWork);
        Task<bool> MarkSpecialEventAssignmentFailedAsync(Guid waitingPostPositionId, string message, int? statusCode = null, CancellationToken cancellationToken = default);
        Task<List<WaitingPostPositionDto>> GetWaitingPostPositionsByEmployeeAndDateRangeAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, Guid? departmentId = null);

        /// <summary>
        /// Renumbers <see cref="PostPositionStatus.Waiting"/> rows for a post and work date so <c>OrderNumber</c> is 1..n
        /// in tenure order: first by <paramref name="orderedEmployeeIds"/> (first occurrence per employee), then any
        /// waiting employees not listed, ordered stably by employee and existing row keys.
        /// </summary>
        Task ReorderWaitingPositionsByGivenOrderNumbersAsync(Guid postId, DateOnly dateOfWork, List<Guid> orderedEmployeeIds, CancellationToken cancellationToken = default);
    }
}

