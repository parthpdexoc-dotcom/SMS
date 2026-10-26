using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for VehicleApprovalRouting entity.
    /// </summary>
    public class VehicleApprovalRoutingDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the vehicle approval route.
        /// </summary>
        public Guid? VehicleApprovalRouteID { get; set; }

        /// <summary>
        /// Gets or sets the approver ID (references RoutingGroup table).
        /// </summary>
        public Guid ApproverID { get; set; }

        /// <summary>
        /// Gets or sets the department equipment ID (references DepartmentEquipment table).
        /// </summary>
        public Guid DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Gets or sets the rank/sequence order for this approval route (1-based).
        /// Rank 1 is the first approver, Rank 2 is the second, etc.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether this approver has final authority.
        /// </summary>
        public bool VehicleApproverHasFinalAuthority { get; set; }
        public RoutingGroupDto? Approver { get; set; }
    }
    public class VehicleApprovalRoutingResponseDto : VehicleApprovalRoutingDto
    {
        public string RoutingGroupName { get; set; }
    }
}

