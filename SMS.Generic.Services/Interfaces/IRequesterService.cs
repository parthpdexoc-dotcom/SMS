using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester service operations.
    /// </summary>
    public interface IRequesterService : IGenericService<RequesterDto, Requester>
    {
        Task<PagedList<RequesterResponseDto>> GetAllRequesterWithDetailsAsync(PageParam pageParam);

        /// <summary>
        /// Requester request summaries (name, request count, project numbers) for requesters visible to the caller.
        /// </summary>
        Task<IReadOnlyList<RequesterRequestSummaryDto>> GetRequesterRequestSummariesAsync(
            RequesterRequestSummaryFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}

