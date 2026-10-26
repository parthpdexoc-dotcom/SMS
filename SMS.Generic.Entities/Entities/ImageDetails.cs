using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an image details entity in the system.
    /// </summary>
    [Table("ImageDetails", Schema = "dbo")]
    public class ImageDetails
    {
        /// <summary>
        /// Gets or sets the unique identifier for the image.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }

        /// <summary>
        /// Gets or sets the image name.
        /// </summary>
        [MaxLength(100)]
        public string? ImageName { get; set; }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        [MaxLength(500)]
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the date when the image was uploaded.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? UploadedDate { get; set; }
    }
}

