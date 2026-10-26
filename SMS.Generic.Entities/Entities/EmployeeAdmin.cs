using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the relationship between an employee and an admin type.
    /// </summary>
    [Table("EmployeeAdmin", Schema = "dbo")]
    public class EmployeeAdmin : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid EmployeeAdminID { get; set; }

        /// <summary>
        /// Gets or sets the admin type identifier associated with the employee.
        /// </summary>
        [Required]
        public Guid AdminTypeID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier associated with the employee.
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Navigation property for the employee.
        /// </summary>
        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// Navigation property for the admin type.
        /// </summary>
        [ForeignKey(nameof(AdminTypeID))]
        public virtual AdminType? AdminType { get; set; }
    }
}
