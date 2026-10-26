using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using System.Linq.Expressions;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for PermitApprovalRouting service operations.
    /// </summary>
    public interface IPermitApprovalRoutingService : IGenericService<PermitApprovalRoutingDto, PermitApprovalRouting>
    {
        /// <summary>
        /// Replaces all permit approval routing records for the given request with the provided list.
        /// UI sends multiple records with the same RequestID, e.g. (RequestID, RoutingGroupId1), (RequestID, RoutingGroupId2).
        /// Existing records for the RequestID are deleted; then the new records are inserted.
        /// </summary>
        /// <param name="dtos">Collection of DTOs with the same RequestID and one RoutingGroupId each.</param>
        /// <returns>The inserted permit approval routing DTOs.</returns>
        Task<IEnumerable<PermitApprovalRoutingDto>> BulkUpsertByRequestIdAsync(IEnumerable<PermitApprovalRoutingDto> dtos);

        /// <summary>
        /// Gets all permit approval routing records by request ID with routing group information.
        /// </summary>
        /// <param name="requestId">The request ID to filter by.</param>
        /// <param name="filter">Optional additional filter expression.</param>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>A collection of permit approval routing response DTOs with routing group names.</returns>
        Task<IEnumerable<PermitApprovalRoutingResponseDto>> GetAllByRequestIdAsync(Guid requestId, Expression<Func<PermitApprovalRouting, bool>>? filter = null, PageParam? pagination = null);
    }
}
