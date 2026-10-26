using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Login record operations.
    /// </summary>
    public class LoginRecordDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the login record.
        /// </summary>
        public int LoID { get; set; }

        /// <summary>
        /// Gets or sets the page where the login attempt occurred.
        /// </summary>
        [MaxLength(25, ErrorMessage = "Page cannot exceed 25 characters")]
        public string? Page { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the login attempt.
        /// </summary>
        [MaxLength(50, ErrorMessage = "IP cannot exceed 50 characters")]
        public string? IP { get; set; }

        /// <summary>
        /// Gets or sets the login ID used in the attempt.
        /// </summary>
        [MaxLength(50, ErrorMessage = "LoginID cannot exceed 50 characters")]
        public string? LoginID { get; set; }

        /// <summary>
        /// Gets or sets the password used in the attempt.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Password cannot exceed 50 characters")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets whether the login attempt passed (was successful).
        /// </summary>
        public bool? Passed { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the login attempt occurred.
        /// </summary>
        [Required(ErrorMessage = "LoginTimestamp is required")]
        public DateTime LoginTimestamp { get; set; }
    }
}

