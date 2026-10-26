using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PrerequisiteType operations.
    /// </summary>
    public class PrerequisiteTypeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the prerequisite type.
        /// </summary>
        public int PkPrerequisiteTypeID { get; set; }

        /// <summary>
        /// Gets or sets the title of the prerequisite type.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the prerequisite type.
        /// </summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether this prerequisite type is a textbox.
        /// </summary>
        public bool? IsTextbox { get; set; }

        /// <summary>
        /// Gets or sets whether this prerequisite type is active.
        /// </summary>
        [Required(ErrorMessage = "Active is required")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        [Required(ErrorMessage = "ParentID is required")]
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this record.
        /// </summary>
        [Required(ErrorMessage = "CreatedBy is required")]
        public long CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when this record was created.
        /// </summary>
        [Required(ErrorMessage = "CreateDate is required")]
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
        [MaxLength(500, ErrorMessage = "Tooltip cannot exceed 500 characters")]
        public string? Tooltip { get; set; }
    }
}

