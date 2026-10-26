using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an uploaded file attachment.
    /// </summary>
    [Table("Files", Schema = "dbo")]
    public class UploadFile : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the uploaded file record (primary key).
        /// </summary>
        [Key]
        public Guid FileUploadedId { get; set; }

         /// <summary>
        /// Navigation property for the parent entity.
        /// </summary>
        [ForeignKey(nameof(RequestID))]
        public virtual Request? Request { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the file type/extension (e.g. pdf, xlsx, txt, docx, jpg, png).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TypeOfFile { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the parent entity this file belongs to (FK to Request).
        /// </summary>
        [Required]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes.
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// Gets or sets the raw file content stored as bytes.
        /// </summary>
        public byte[]? FileContent { get; set; }

       
    }
}
