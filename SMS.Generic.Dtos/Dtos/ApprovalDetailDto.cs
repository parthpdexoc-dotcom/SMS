using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ApprovalDetail (ApprovalDetails table) operations.
    /// </summary>
    public class ApprovalDetailDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the approval detail.
        /// </summary>
        public Guid? ApprovalDetailID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier (foreign key to Request).
        /// </summary>
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the route-to identifier (represents the approver/route destination).
        /// </summary>
        public Guid RouteToID { get; set; }

        /// <summary>
        /// Gets or sets comments from the approver.
        /// </summary>
        public string? VehicleApproverComments { get; set; }

        /// <summary>
        /// Gets or sets whether this approval step has been approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets whether this approval represents the final approval for the request.
        /// </summary>
        public bool FinalApproval { get; set; }

        /// <summary>
        /// Gets or sets the approval type.
        /// </summary>
        public ApprovalType Type { get; set; }
    }
}
