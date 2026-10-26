using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeContactType operations.
    /// </summary>
    public class EmployeeContactTypeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee contact type.
        /// </summary>
        public int PkEmployeeContactTypeID { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(500, ErrorMessage = "Title cannot exceed 500 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether this is a textbox.
        /// </summary>
        public bool? IsTextbox { get; set; }

        /// <summary>
        /// Gets or sets whether this contact type is active.
        /// </summary>
        [Required(ErrorMessage = "Active is required")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        [Required(ErrorMessage = "ParentID is required")]
        public long ParentID { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created this record.
        /// </summary>
        [Required(ErrorMessage = "CreatedBy is required")]
        public long CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [Required(ErrorMessage = "CreateDate is required")]
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
        [MaxLength(500, ErrorMessage = "Tooltip cannot exceed 500 characters")]
        public string? Tooltip { get; set; }
    }
}

