using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester contact type entity in the system.
    /// </summary>
    [Table("RequesterContactTypes", Schema = "dbo")]
    public class RequesterContactTypes
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester contact type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKRequesterContactTypeID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether this is a textbox.
        /// </summary>
        public bool? IsTextbox { get; set; }

        /// <summary>
        /// Gets or sets whether this contact type is active.
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        [Required]
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created this record.
        /// </summary>
        [Required]
        public long CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }

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

        /// <summary>
        /// Gets or sets the tooltip text.
        /// </summary>
        [MaxLength(500)]
        public string? Tooltip { get; set; }
    }
}

