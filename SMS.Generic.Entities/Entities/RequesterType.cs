using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester type entity in the system.
    /// </summary>
    [Table("RequesterType", Schema = "dbo")]
    public class RequesterType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PkRequesterTypeID { get; set; }

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
        /// Gets or sets whether this is a textbox type.
        /// </summary>
        public bool? IsTextbox { get; set; }

        /// <summary>
        /// Gets or sets whether the requester type is active.
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        [Required]
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the user who created the record.
        /// </summary>
        [Required]
        public long CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets whether the record is deleted.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets the user who deleted the record.
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

