using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequestApprovalRouting entity.
    /// </summary>
    public class RequestApprovalRoutingDto : BaseDto
    {
        public RequestApprovalRoutingDto()
        {
        }

        public RequestApprovalRoutingDto(Guid ApproverID,Guid RequestID, int Rank)
        {
            this.ApproverID = ApproverID;
            this.RequestID = RequestID;
            this.Rank = Rank;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the request approval route.
        /// </summary>
        public Guid? RequestApprovalRouteID { get; set; }

        /// <summary>
        /// Gets or sets the approver ID (references RoutingGroup table).
        /// </summary>
        public Guid ApproverID { get; set; }

        /// <summary>
        /// Gets or sets the request ID (references Request table).
        /// NOTE: Migration shows DepartmentEquipmentID, but this should be RequestID.
        /// </summary>
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the rank/sequence order for this approval route (1-based).
        /// Rank 1 is the first approver, Rank 2 is the second, etc.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether this approver has final authority.
        /// </summary>
        public bool RequestApproverHasFinalAuthority { get; set; }
        public RoutingGroupDto? Approver { get; set; }
    }

    /// <summary>
    /// Response DTO for RequestApprovalRouting with routing group name.
    /// </summary>
    public class RequestApprovalRoutingResponseDto : RequestApprovalRoutingDto
    {
        /// <summary>
        /// Gets or sets the routing group name (from Approver navigation property).
        /// </summary>
        public string? RoutingGroupName { get; set; }

        /// <summary>
        /// Gets or sets the approval type for the routing group.
        /// </summary>
        public ApprovalType ApprovalType { get; set; }
    }
}

