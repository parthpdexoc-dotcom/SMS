using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmailAccessCode operations.
    /// </summary>
    public class EmailAccessCodeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier (serial number).
        /// </summary>
        public int SL { get; set; }

        /// <summary>
        /// Gets or sets the access code.
        /// </summary>
        [MaxLength(100, ErrorMessage = "AccessCode cannot exceed 100 characters")]
        public string? AccessCode { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time.
        /// </summary>
        public DateTime? CreatedDTS { get; set; }

        /// <summary>
        /// Gets or sets whether the code is valid.
        /// </summary>
        public bool? IsValidCode { get; set; }
    }
}

