using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester restricted list entity in the system.
    /// Tracks employees that are NOT allowed to work for specific requester.
    /// </summary>
    [Table("RequesterRestrictedList", Schema = "dbo")]
    public class RequesterRestrictedList:BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester restricted list record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // because NEWID() is in SQL, not EF
        public Guid RequesterRestrictID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (foreign key to Requesters).
        /// </summary>
        [Required]
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier (foreign key to Employee).
        /// ID of employee NOT allowed to work for this Requester.
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the effective start date for this restriction.
        /// </summary>
        [Column(TypeName = "date")]
        public DateOnly EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the effective end date for this restriction.
        /// </summary>
        [Column(TypeName = "date")]
        public DateOnly EffectiveEndDate { get; set; }
        /// <summary>
        /// Navigation property to the requester.
        /// </summary>
        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Navigation property to the employee (restricted employee).
        /// </summary>
        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        /// <summary>
        /// Navigation property to the admin who entered this record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? EnteredByEmployee { get; set; }

        /// <summary>
        /// Navigation property to the admin who last updated this record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? UpdatedByEmployee { get; set; }
    }
}

