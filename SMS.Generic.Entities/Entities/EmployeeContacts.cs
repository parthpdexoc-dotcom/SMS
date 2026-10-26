using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an employee contact entity in the system.
    /// </summary>
    [Table("EmployeeContacts", Schema = "dbo")]
    public class EmployeeContacts
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee contact.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContactID { get; set; }

        /// <summary>
        /// Gets or sets the contact type ID.
        /// </summary>
        public int? ContactTypeID { get; set; }

        /// <summary>
        /// Gets or sets the contact at information.
        /// </summary>
        [MaxLength(50)]
        public string? ContactAt { get; set; }

        /// <summary>
        /// Gets or sets the email ID.
        /// </summary>
        [MaxLength(50)]
        public string? EmailID { get; set; }

        /// <summary>
        /// Gets or sets whether this is the primary contact.
        /// </summary>
        public bool? IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets whether this contact is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created this record.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last modified this record.
        /// </summary>
        public int? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID who deleted this record.
        /// </summary>
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the deletion date.
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets the foreign key employee ID (EmployeeID as Guid).
        /// </summary>
        public Guid? FkEmployeeID { get; set; }
    }
}

