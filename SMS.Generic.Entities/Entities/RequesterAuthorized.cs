using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an authorized requester contact for a requester.
    /// </summary>
    [Table("RequesterAuthorized", Schema = "dbo")]
    public class RequesterAuthorized : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the authorized requester record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequesterAuthorizedID { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (foreign key to Requesters).
        /// </summary>
        [Required]
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the first name of the authorized requester.
        /// </summary>
        [MaxLength(50)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the authorized requester.
        /// </summary>
        [MaxLength(50)]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the primary phone number of the authorized requester.
        /// </summary>
        [MaxLength(50)]
        public string? PrimaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the secondary phone number of the authorized requester.
        /// </summary>
        [MaxLength(50)]
        public string? SecondaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the primary email address of the authorized requester.
        /// </summary>
        [MaxLength(50)]
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets the secondary email address of the authorized requester.
        /// </summary>
        [MaxLength(50)]
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// Gets or sets the status of this authorized requester.
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// Navigation property to the requester.
        /// </summary>
        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Navigation property to the employee who added the record.
        /// Uses BaseEntity.CreatedUserID.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? AddedByEmployee { get; set; }

        /// <summary>
        /// Navigation property to the employee who last edited the record.
        /// Uses BaseEntity.EditedUserID.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}


