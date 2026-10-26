using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a prerequisite type entity in the system.
    /// </summary>
    [Table("PrerequisiteTypes", Schema = "dbo")]
    public class PrerequisiteType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the prerequisite type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PkPrerequisiteTypeID { get; set; }

        /// <summary>
        /// Gets or sets the title of the prerequisite type.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the prerequisite type.
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether this prerequisite type is a textbox.
        /// </summary>
        public bool? IsTextbox { get; set; }

        /// <summary>
        /// Gets or sets whether this prerequisite type is active.
        /// </summary>
        [Required]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        [Required]
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this record.
        /// </summary>
        [Required]
        public long CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was created.
        /// </summary>
        [Required]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last modified this record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was last modified.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets whether this record is deleted.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who deleted this record.
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was deleted.
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        /// <summary>
        /// Gets or sets the tooltip text for this prerequisite type.
        /// </summary>
        [MaxLength(500)]
        public string? Tooltip { get; set; }
    }
}

