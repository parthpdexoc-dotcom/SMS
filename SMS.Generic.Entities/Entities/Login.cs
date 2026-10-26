using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a login record entity in the system.
    /// </summary>
    [Table("Login", Schema = "dbo")]
    public class Login
    {
        /// <summary>
        /// Gets or sets the unique identifier for the login record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoID { get; set; }

        /// <summary>
        /// Gets or sets the page where the login attempt occurred.
        /// </summary>
        [MaxLength(25)]
        public string? Page { get; set; }

        /// <summary>
        /// Gets or sets the IP address of the login attempt.
        /// </summary>
        [MaxLength(50)]
        public string? IP { get; set; }

        /// <summary>
        /// Gets or sets the login ID used in the attempt.
        /// </summary>
        [MaxLength(50)]
        public string? LoginID { get; set; }

        /// <summary>
        /// Gets or sets the password used in the attempt.
        /// </summary>
        [MaxLength(50)]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets whether the login attempt passed (was successful).
        /// </summary>
        public bool? Passed { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the login attempt occurred.
        /// </summary>
        [Required]
        public DateTime LoginTimestamp { get; set; }
    }
}

