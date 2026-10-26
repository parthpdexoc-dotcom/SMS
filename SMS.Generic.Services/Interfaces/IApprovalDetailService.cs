using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for ApprovalDetail service operations (ApprovalDetails table).
    /// </summary>
    public interface IApprovalDetailService : IGenericService<ApprovalDetailDto, ApprovalDetail>
    {
        /// <summary>
        /// Gets the approval request detail for the approval page by requestId, routingGroupId, and approvalType.
        /// Returns null if request not found or routing group is not in the request's approval routings.
        /// </summary>
        Task<ApprovalRequestDetailResponseDto?> GetApprovalRequestDetailAsync(Guid requestId, Guid routingGroupId, ApprovalType? approvalType, CancellationToken cancellationToken = default);
    }
}
