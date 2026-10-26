using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Tracks an employee's tenure, department, type, and key dates.
    /// </summary>
    [Table("EmployeeTenure", Schema = "dbo")]
    public class EmployeeTenure : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeTenureID { get; set; }

        [Required]
        public Guid DepartmentID { get; set; }

        [Required]
        public Guid EmployeeID { get; set; }

        [Required]
        public Guid EmployeeTypeID { get; set; }

        public DateOnly? DateOfHire { get; set; }

        public DateOnly? DateOfPromotion { get; set; }

        /// <summary>
        /// Indicates if this is the current tenure record for the employee.
        /// Only one record per employee can have IsCurrent = true.
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Navigation to department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation to employee (subject).
        /// </summary>
        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// Navigation to employee type.
        /// </summary>
        [ForeignKey(nameof(EmployeeTypeID))]
        public virtual EmployeeType? EmployeeType { get; set; }

        /// <summary>
        /// Navigation to employee who added the record (CreatedUserID).
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? AddedByEmployee { get; set; }

        /// <summary>
        /// Navigation to employee who last edited the record (EditedUserID).
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}


