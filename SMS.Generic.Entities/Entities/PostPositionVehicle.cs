using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a vehicle assigned to a post position with optional charge overrides.
    /// </summary>
    [Table("PostPositionVehicle", Schema = "dbo")]
    public class PostPositionVehicle : BaseEntity
    {
        /// <summary>
        /// Primary Key - Post Position Vehicle ID (GUID).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostPositionVehicleID { get; set; }

        /// <summary>
        /// Foreign Key - Post Position ID.
        /// </summary>
        [Required]
        public Guid PostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID.
        /// </summary>
        [Required]
        public Guid DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Charged type (per hour / per item).
        /// </summary>
        public int? ChargedType { get; set; }

        /// <summary>
        /// Base charge amount.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? Charge { get; set; }

        /// <summary>
        /// Whether mileage is charged.
        /// </summary>
        public bool? ChargeMileage { get; set; }

        /// <summary>
        /// Money value per mile.
        /// </summary>
        [Column(TypeName = "money")]
        public decimal? MileageCharge { get; set; }

        /// <summary>
        /// Mileage condition.
        /// </summary>
        public int? MileageCondition { get; set; }

        /// <summary>
        /// Equipment quantity assigned (frozen at shift freeze / start).
        /// </summary>
        public int? EquipmentQuantity { get; set; }

        /// <summary>
        /// Job type frozen at shift freeze / start.
        /// When true, Per Position (every filled position gets one; quantity and rank are not used).
        /// When false, Per Job / Per Shift (rank then hire date, up to quantity).
        /// Same semantics as <see cref="PostEquipment.EquipmentPerPostPosition"/>.
        /// </summary>
        public bool EquipmentPerPostPosition { get; set; } 

        /// <summary>
        /// Navigation property for the post position.
        /// </summary>
        [ForeignKey(nameof(PostPositionID))]
        public virtual PostPosition? PostPosition { get; set; }

        /// <summary>
        /// Navigation property for the department equipment.
        /// </summary>
        [ForeignKey(nameof(DepartmentEquipmentID))]
        public virtual DepartmentEquipment? DepartmentEquipment { get; set; }

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
