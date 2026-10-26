using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the equipment assigned to a post.
    /// </summary>
    [Table("PostEquipment", Schema = "dbo")]
    public class PostEquipment : BaseEntity
    {
        /// <summary>
        /// Primary Key - Post Equipment ID (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PostEquipmentID { get; set; }

        /// <summary>
        /// Foreign Key - Post ID
        /// </summary>
        [Required]
        public Guid PostID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID
        /// </summary>
        [Required]
        public Guid DepartmentEquipmentID { get; set; }

        /// <summary>
        /// When true, Per Job Position (every filled position gets one; quantity and rank are not used).
        /// When false, Per Job (rank then hire date, up to EquipmentQuantity, one per position).
        /// </summary>
        public bool EquipmentPerPostPosition { get; set; }

        /// <summary>
        /// Equipment quantity
        /// </summary>
        public int EquipmentQuantity { get; set; }

        /// <summary>
        /// Indicates whether the equipment is a vehicle (true/false)
        /// </summary>
        public bool IsVehicle { get; set; }

        /// <summary>
        /// Equipment effective start date for this post assignment.
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Equipment effective end date for this post assignment.
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Navigation property for the post.
        /// </summary>
        [ForeignKey(nameof(PostID))]
        public virtual Post? Post { get; set; }

        /// <summary>
        /// Navigation property for the department equipment.
        /// </summary>
        [ForeignKey(nameof(DepartmentEquipmentID))]
        public virtual DepartmentEquipment? DepartmentEquipment { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this post equipment.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this post equipment.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
