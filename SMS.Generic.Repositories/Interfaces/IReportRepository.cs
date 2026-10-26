using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Repositories.Interfaces;

/// <summary>
/// Read-side queries for operator reports: CYA archived rows (<see cref="ReportRowDto"/>) and watch commander post-position rows (<see cref="WatchCommanderReportDto"/>).
/// </summary>
public interface IReportRepository
{
    /// <summary>
    /// Returns a paged list of report rows joined from <c>ClosedPostPosition</c> → <c>PostPosition</c> → <c>Employee</c>, <c>Post</c>, <c>Request</c>, <c>Requester</c>, pay/response area, and vehicle equipment.
    /// Rows are limited to employees in <paramref name="departmentId"/> (<c>Employee.DepartmentID</c>).
    /// Optional <paramref name="filterStartDate"/> / <paramref name="filterEndDate"/> filter on <c>PostPosition.DateOfWork</c> when set, otherwise <c>ClosedPostPosition.WorkStartDate</c>.
    /// Optional <paramref name="postId"/> limits rows to <c>Post.PostID</c>.
    /// When <paramref name="includeAllMatchingRows"/> is <c>true</c>, every matching row is returned (same filters); <paramref name="pageParam"/> skip/take are ignored.
    /// In that mode the total is derived from the loaded set (no separate count query) to reduce database work.
    /// </summary>
    Task<PagedList<ReportRowDto>> GetReportRowsAsync(
        Guid departmentId,
        DateOnly? filterStartDate,
        DateOnly? filterEndDate,
        Guid? postId,
        string? searchTerm,
        PageParam pageParam,
        bool isGenerateReport = false,
        bool? isPlaceholder = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Watch commander report: <see cref="SMS.Generic.Enums.PostPositionStatus.Assigned"/> and <see cref="SMS.Generic.Enums.PostPositionStatus.CancellationPending"/> post positions for employees in <paramref name="departmentId"/>,
    /// optional <see cref="SMS.Generic.Entities.Entities.PostPosition.DateOfWork"/> filter, optional <paramref name="searchTerm"/> (employee name / &quot;Officer Name&quot;), server-side sort and projection. Hours come from <see cref="SMS.Generic.Entities.Entities.Post.PostLength"/> (minutes ÷ 60), not closed-post data.
    /// When <paramref name="isGenerateReport"/> is true, returns every matching row (skip/take ignored).
    /// </summary>
    Task<PagedList<WatchCommanderReportDto>> GetWatchCommanderReportPagedAsync(
        Guid departmentId,
        DateOnly? startDate,
        DateOnly? endDate,
        string? searchTerm,
        PageParam? pageParam,
        bool isGenerateReport = false,
        CancellationToken cancellationToken = default);
}
