using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the equipment assigned to a request.
    /// </summary>
    [Table("RequestEquipment", Schema = "dbo")]
    public class RequestEquipment : BaseEntity
    {
        /// <summary>
        /// Primary Key - Request Equipment ID (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestEquipmentID { get; set; }

        /// <summary>
        /// Foreign Key - Request ID
        /// </summary>
        [Required]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID
        /// </summary>
        [Required]
        public Guid DepartmentEquipmentID { get; set; }

        /// <summary>
        /// When true, Per Job (rank then hire date, up to EquipmentQuantity, one per position).
        /// When false, Per Job Position (every filled position gets one; quantity and rank are not used).
        /// </summary>
        public bool EquipmentPerPost { get; set; }

        /// <summary>
        /// Equipment quantity
        /// </summary>
        public int EquipmentQuantity { get; set; }

        /// <summary>
        /// Indicates whether the equipment is a vehicle (true/false)
        /// </summary>
        public bool IsVehicle { get; set; }

        /// <summary>
        /// Equipment effective start date for this request assignment.
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Equipment effective end date for this request assignment.
        /// </summary>
        [Required]
        [Column(TypeName = "date")]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Navigation property for the request.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

        /// <summary>
        /// Navigation property for the department equipment.
        /// </summary>
        [ForeignKey(nameof(DepartmentEquipmentID))]
        public virtual DepartmentEquipment? DepartmentEquipment { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this request equipment.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this request equipment.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
