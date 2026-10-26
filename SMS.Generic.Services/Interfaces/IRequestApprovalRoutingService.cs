using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for RequestApprovalRouting service operations.
    /// </summary>
    public interface IRequestApprovalRoutingService : IGenericService<RequestApprovalRoutingDto, RequestApprovalRouting>
    {
        /// <summary>
        /// Upserts multiple request approval routing records with rank-based sequencing.
        /// </summary>
        /// <param name="dtos">The collection of DTOs to upsert.</param>
        /// <returns>A collection of upserted DTOs.</returns>
        /// <param name="isSuperUser">When true, audit fields on each DTO are not auto-populated (see <c>IGenericService</c>).</param>
        Task<IEnumerable<RequestApprovalRoutingDto>> BulkUpsertAsync(IEnumerable<RequestApprovalRoutingDto> dtos, bool isSuperUser = false);

        /// <summary>
        /// Gets all request approval routing records by request ID with routing group information.
        /// </summary>
        /// <param name="requestId">The request ID to filter by.</param>
        /// <param name="filter">Optional additional filter expression.</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A collection of request approval routing response DTOs with routing group names.</returns>
        Task<IEnumerable<RequestApprovalRoutingResponseDto>> GetAllRequestRoutingGroupByRequestIdAsync(Guid requestId, Expression<Func<RequestApprovalRouting, bool>>? filter = null, PageParam? pagination = null);
    }
}

