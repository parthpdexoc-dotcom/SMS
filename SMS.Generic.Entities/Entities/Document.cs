using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a document entity in the system.
    /// </summary>
    [Table("Documents")]
    public class Document
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the document.
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the file name of the document.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file path where the document is stored.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the MIME type of the document.
        /// </summary>
        [MaxLength(100)]
        public string? MimeType { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the category of the document.
        /// </summary>
        [MaxLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the tags associated with the document.
        /// </summary>
        [MaxLength(500)]
        public string? Tags { get; set; }

        /// <summary>
        /// Gets or sets the version of the document.
        /// </summary>
        [MaxLength(20)]
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Gets or sets a value indicating whether the document is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date and time when the document was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the document was last updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created the document.
        /// </summary>
        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last updated the document.
        /// </summary>
        [MaxLength(50)]
        public string? UpdatedBy { get; set; }
    }
}
