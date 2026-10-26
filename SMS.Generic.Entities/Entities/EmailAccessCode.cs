using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an email access code entity in the system.
    /// </summary>
    [Table("EmailAccessCode", Schema = "dbo")]
    public class EmailAccessCode
    {
        /// <summary>
        /// Gets or sets the unique identifier (serial number).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SL { get; set; }

        /// <summary>
        /// Gets or sets the access code.
        /// </summary>
        [MaxLength(100)]
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

