using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents vehicle approval routing configuration with rank-based sequencing.
    /// Records are ordered by Rank (1, 2, 3, ...) where Rank 1 is the first approver.
    /// </summary>
    [Table("VehicleApprovalRouting", Schema = "dbo")]
    public class VehicleApprovalRouting : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the vehicle approval route.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid VehicleApprovalRouteID { get; set; }

        /// <summary>
        /// Gets or sets the approver ID (references RoutingGroup table).
        /// </summary>
        [Required]
        public Guid ApproverID { get; set; }

        /// <summary>
        /// Gets or sets the department equipment ID (references DepartmentEquipment table).
        /// </summary>
        [Required]
        public Guid DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Gets or sets the rank/sequence order for this approval route (1-based).
        /// Rank 1 is the first approver, Rank 2 is the second, etc.
        /// </summary>
        [Required]
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether this approver has final authority.
        /// </summary>
        public bool VehicleApproverHasFinalAuthority { get; set; }

        // Navigation properties
        /// <summary>
        /// Navigation property for the approver routing group.
        /// </summary>
        [ForeignKey(nameof(ApproverID))]
        public virtual RoutingGroup? Approver { get; set; }

        /// <summary>
        /// Navigation property for the department equipment.
        /// </summary>
        [ForeignKey(nameof(DepartmentEquipmentID))]
        public virtual DepartmentEquipment? DepartmentEquipment { get; set; }

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

