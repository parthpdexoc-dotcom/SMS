using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department admin entity in the system.
    /// </summary>
    [Table("DepartmentAdmin", Schema = "dbo")]
    public class DepartmentAdmin : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department admin.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentAdminID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the admin type identifier.
        /// </summary>
        [Required]
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee.
        /// </summary>
        [ForeignKey("EmployeeID")]
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// Navigation property for the admin type.
        /// </summary>
        [ForeignKey("AdminTypeID")]
        public virtual AdminType? AdminType { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this record.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this record.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
