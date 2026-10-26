using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ExportLog operations.
    /// </summary>
    public class ExportLogDto
    {
        /// <summary>
        /// Gets or sets the unique identifier (export ID).
        /// </summary>
        public int ExportID { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string? Status { get; set; }
    }
}

