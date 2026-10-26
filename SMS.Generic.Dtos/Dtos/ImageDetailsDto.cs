using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ImageDetails operations.
    /// </summary>
    public class ImageDetailsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the image.
        /// </summary>
        public int ImageID { get; set; }

        /// <summary>
        /// Gets or sets the image name.
        /// </summary>
        [MaxLength(100, ErrorMessage = "ImageName cannot exceed 100 characters")]
        public string? ImageName { get; set; }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        [MaxLength(500, ErrorMessage = "ImagePath cannot exceed 500 characters")]
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the date when the image was uploaded.
        /// </summary>
        public DateTime? UploadedDate { get; set; }
    }
}

