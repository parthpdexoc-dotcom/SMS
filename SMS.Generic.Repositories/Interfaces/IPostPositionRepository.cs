using System.Linq.Expressions;
using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for post repository operations, extending IGenericRepository.
    /// </summary>
    public interface IPostPositionRepository : IGenericRepository<PostPosition>
    {
        /// <summary>
        /// Post positions in the assigned / split-pending / unclosed / cancellation-pending family with <see cref="PostPosition.DateOfWork"/> set,
        /// optionally filtered by employee and/or request department (same query shape as employee scheduled/ongoing lists).
        /// </summary>
        Task<List<PostPosition>> GetAssignedWithDateOfWorkAsync(
            Guid? employeeId,
            bool restrictToEmployee,
            Guid? requestDepartmentId,
            IReadOnlyCollection<PostPositionStatus> statuses,
            Expression<Func<PostPosition, object>>[]? includes);

        Task<List<PostPosition>> GetPostPositionByEmployeeAndDateRangeAsync(
            Guid employeeId,
            DateOnly startDate,
            DateOnly endDate,
            Guid? departmentId = null,
            bool excludeCancelledOnly = true,
            bool enableExemptRequestsJobsFromHoursRules = false);
        Task<int> GetPostsTakenLast24HoursAsync(Guid employeeId, DateTime dateTime);
        Task<bool> TransferSourceAsync(Guid sourcePostPositionId, Guid? editedUserId, string? editedBy);
        Task<bool> SetClosedAndSplitStatusAsync(Guid sourcePostPositionId, Guid? editedUserId, string? editedBy);
        /// <summary>
        /// Updates status via ExecuteUpdate, bypassing the change tracker when another scope may have modified the row.
        /// </summary>
        Task<int> UpdatePostPositionStatusDirectAsync(Guid postPositionId, PostPositionStatus status);

        /// <summary>
        /// Cancels multiple post positions in one round-trip.
        /// </summary>
        Task<int> CancelPostPositionsByIdsAsync(
            IReadOnlyCollection<Guid> postPositionIds,
            CancellationToken cancellationToken = default);

        Task<PostPosition?> AssignTransferredPostAsync(
            Guid targetEmployeeId,
            Guid postId,
            DateOnly dateOfWork,
            TimeOnly postStartTime,
            TimeOnly postEndTime,
            int postLength,
            string? callSign,
            Guid? createdUserId,
            string? createdBy,
            bool enableInvoiceAndPayroll = false);

        /// <summary>
        /// Assigned / cancellation-pending post positions for a department and date window, including
        /// employee and post graph needed by officer schedule overview.
        /// </summary>
        Task<IReadOnlyList<PostPosition>> GetOfficerSchedulePostPositionsAsync(
            Guid departmentId,
            DateOnly minDateOfWork,
            DateOnly maxDateOfWork,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Real-employee assigned / cancellation-pending post positions for a one-time request on a specific work date.
        /// </summary>
        Task<List<PostPosition>> GetAssignedRealEmployeesByRequestAndDateAsync(
            Guid requestId,
            DateOnly dateOfWork,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Lightweight projections for one-time request date-change assignment review and conflict checks.
        /// </summary>
        Task<OneTimeDateChangeReviewDataDto> GetOneTimeDateChangeReviewDataAsync(
            Guid requestId,
            DateOnly oldDate,
            DateOnly newDate,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigned post positions deduplicated to one row per recipient email, paged at the database
        /// with the same Offset/Limit semantics as <see cref="IGenericRepository{T}"/> paging.
        /// </summary>
        Task<(List<PostPosition> Rows, int Total)> GetAssignedRecipientsPagedAsync(
            Expression<Func<PostPosition, bool>> filter,
            PageParam pageParam);

        /// <summary>
        /// Distinct <see cref="PostPosition.DateOfWork"/> values for a post, ordered ascending.
        /// </summary>
        Task<List<DateOnly>> GetDistinctDatesOfWorkByPostIdAsync(
            Guid postId,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Post-position rows for a post (daily-admin statuses), with employee/placeholder names.
        /// Date filter, sort, and pagination run on the database.
        /// </summary>
        Task<PagedList<PostPositionInRequestListDto>> GetByPostFilterAsync(
            Guid postId,
            DateOnly? startDate = null,
            DateOnly? endDate = null,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Post positions for a post with only the employee, placeholder, call-sign, and
        /// request-schedule columns used by the daily list. Applies the same request-status filter
        /// as <c>GetPostPositionsWithEmployeeAsync</c>.
        /// </summary>
        Task<List<PostPositionResponseDto>> GetPostPositionsWithEmployeeColumnsAsync(
            Guid postId,
            DateOnly? dateOfWork,
            IReadOnlyCollection<DateOnly>? datesOfWork,
            IReadOnlyCollection<RequestStatus>? requestStatuses,
            bool includeCompleted,
            PageParam? pagination = null,
            CancellationToken cancellationToken = default);

        Task<bool> SwapCallSignAsync(Guid postPositionId1, Guid postPositionId2);

        /// <summary>
        /// Loads a post position for Job Position Flow read, including soft-deleted
        /// <see cref="PostPosition"/>, <see cref="Post"/>, and <see cref="Employee"/> rows.
        /// </summary>
        Task<PostPosition?> GetByIdForFlowAuditAsync(
            Guid postPositionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Assigned, split-pending, unclosed, and cancellation-pending jobs for one employee on one work date,
        /// ordered by start time, then end time. Request is joined only when <paramref name="departmentId"/> is set.
        /// </summary>
        Task<List<EmployeePostListingItemDto>> GetTodaysJobsAsync(
            Guid employeeId,
            DateOnly dateOfWork,
            Guid? departmentId,
            CancellationToken cancellationToken = default);
    }
}

