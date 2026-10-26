using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents the mapping between a requester and an employee who manages private jobs.
    /// </summary>
    [Table("RequesterManager", Schema = "dbo")]
    public class RequesterManager : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester manager record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequesterManagerID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (foreign key to Requesters).
        /// </summary>
        [Required]
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier (foreign key to Employee).
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Indicates whether this manager assignment is shared.
        /// </summary>
        public bool IsEmployeeShared { get; set; }
        /// <summary>
        /// Indicates whether this manager request is shared.
        /// </summary>
        public bool IsRequestShared { get; set; }

        /// <summary>
        /// Navigation property to the requester.
        /// </summary>
        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Navigation property to the employee (manager).
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
        public virtual ICollection<RequesterEmployee> RequesterEmployees { get; set; } = new List<RequesterEmployee>();
    }
}


