using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester contact entity in the system.
    /// </summary>
    [Table("RequesterContacts", Schema = "dbo")]
    public class RequesterContacts
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester contact.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PkContactID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key department ID.
        /// </summary>
        public Guid? FkDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key requester ID (references Requesters table).
        /// </summary>
        public Guid? FkRequesterID { get; set; }

        /// <summary>
        /// Navigation property for the requester.
        /// </summary>
        [ForeignKey(nameof(FkRequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Gets or sets the foreign key contact type ID.
        /// </summary>
        public int? FkContactTypeID { get; set; }

        /// <summary>
        /// Gets or sets the contact name.
        /// </summary>
        [MaxLength(255)]
        public string? ContactName { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the phone extension.
        /// </summary>
        [MaxLength(5)]
        public string? PhoneExt { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public short? Status { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created this record.
        /// </summary>
        public long? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last modified this record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets whether this record is deleted.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets the user ID who deleted this record.
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the deletion date.
        /// </summary>
        public DateTime? DeletedDate { get; set; }
    }
}

