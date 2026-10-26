using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents permit approval routing configuration linking a request to an approver (routing group).
    /// </summary>
    [Table("PermitApprovalRouting", Schema = "dbo")]
    public class PermitApprovalRouting : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the permit approval route.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PermitApprovalRoutingID { get; set; }

        /// <summary>
        /// Gets or sets the request ID (references Request table).
        /// </summary>
        [Required]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the routing group ID (references RoutingGroup table).
        /// </summary>
        [Required]
        public Guid RoutingGroupId { get; set; }

        /// <summary>
        /// Navigation property for the request.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

        /// <summary>
        /// Navigation property for the approver routing group.
        /// </summary>
        [ForeignKey(nameof(RoutingGroupId))]
        public virtual RoutingGroup? Approver { get; set; }

        /// <summary>
        /// Navigation property for the employee who created the record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who edited the record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
