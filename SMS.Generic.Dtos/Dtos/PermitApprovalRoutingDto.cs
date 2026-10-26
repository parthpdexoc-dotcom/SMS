using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PermitApprovalRouting entity.
    /// </summary>
    public class PermitApprovalRoutingDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the permit approval route.
        /// </summary>
        public Guid? PermitApprovalRoutingID { get; set; }

        /// <summary>
        /// Gets or sets the request ID (references Request table).
        /// </summary>
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the routing group ID (references RoutingGroup table).
        /// </summary>
        public Guid RoutingGroupId { get; set; }
    }

    /// <summary>
    /// Response DTO for PermitApprovalRouting with routing group name.
    /// </summary>
    public class PermitApprovalRoutingResponseDto : PermitApprovalRoutingDto
    {
        /// <summary>
        /// Gets or sets the routing group name (from Approver navigation property).
        /// </summary>
        public string? RoutingGroupName { get; set; }
    }
}
