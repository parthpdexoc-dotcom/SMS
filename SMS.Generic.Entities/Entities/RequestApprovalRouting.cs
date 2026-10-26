using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents request approval routing configuration with rank-based sequencing.
    /// Records are ordered by Rank (1, 2, 3, ...) where Rank 1 is the first approver.
    /// </summary>
    [Table("RequestApprovalRouting", Schema = "dbo")]
    public class RequestApprovalRouting : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request approval route.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestApprovalRouteID { get; set; }

        /// <summary>
        /// Gets or sets the approver ID (references RoutingGroup table).
        /// </summary>
        [Required]
        public Guid ApproverID { get; set; }

        /// <summary>
        /// Gets or sets the request ID (references Request table).
        /// </summary>
        [Required]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the rank/sequence order for this approval route (1-based).
        /// Rank 1 is the first approver, Rank 2 is the second, etc.
        /// </summary>
        [Required]
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether this approver has final authority.
        /// </summary>
        public bool RequestApproverHasFinalAuthority { get; set; }

        // Navigation properties
        /// <summary>
        /// Navigation property for the approver routing group.
        /// </summary>
        [ForeignKey(nameof(ApproverID))]
        public virtual RoutingGroup? Approver { get; set; }

        /// <summary>
        /// Navigation property for the request.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

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

