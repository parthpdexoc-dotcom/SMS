using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an approval detail record for a specific request and department (table: ApprovalDetails).
    /// </summary>
    [Table("ApprovalDetails", Schema = "dbo")]
    public class ApprovalDetail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the approval detail.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApprovalDetailID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        [Required]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier (foreign key to Request).
        /// </summary>
        [Required]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the route-to identifier (represents the approver/route destination).
        /// </summary>
        [Required]
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

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the request.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
