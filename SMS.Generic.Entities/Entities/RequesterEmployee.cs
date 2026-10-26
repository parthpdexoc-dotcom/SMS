using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the mapping between a requester manager and an employee.
    /// </summary>
    [Table("RequesterEmployee", Schema = "dbo")]
    public class RequesterEmployee : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester employee record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequesterEmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the requester manager identifier (foreign key to RequesterManager).
        /// </summary>
        [Required]
        public Guid RequesterManagerID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier (foreign key to Employee).
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets whether this employee is preferred.
        /// </summary>
        public bool IsPreferred { get; set; }

        /// <summary>
        /// Navigation property to the requester manager.
        /// </summary>
        [ForeignKey(nameof(RequesterManagerID))]
        public virtual RequesterManager? RequesterManager { get; set; }

        /// <summary>
        /// Navigation property to the employee.
        /// </summary>
        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// Navigation property to the employee who added the record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? AddedByEmployee { get; set; }

        /// <summary>
        /// Navigation property to the employee who last edited the record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
