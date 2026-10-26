using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Document operations.
    /// </summary>
    public class DocumentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the document.
        /// </summary>
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the file name of the document.
        /// </summary>
        [Required(ErrorMessage = "File name is required")]
        [MaxLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file path where the document is stored.
        /// </summary>
        [Required(ErrorMessage = "File path is required")]
        [MaxLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MIME type of the document.
        /// </summary>
        [MaxLength(100, ErrorMessage = "MIME type cannot exceed 100 characters")]
        public string? MimeType { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        [Range(0, long.MaxValue, ErrorMessage = "File size must be a positive number")]
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the category of the document.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the tags associated with the document.
        /// </summary>
        [MaxLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
        public string? Tags { get; set; }

        /// <summary>
        /// Gets or sets the version of the document.
        /// </summary>
        [MaxLength(20, ErrorMessage = "Version cannot exceed 20 characters")]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the document is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time when the document was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the document was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created the document.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Created by cannot exceed 50 characters")]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last updated the document.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Updated by cannot exceed 50 characters")]
        public string? UpdatedBy { get; set; }
    }
}
