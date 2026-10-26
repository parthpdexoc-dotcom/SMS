using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an export log entity in the system.
    /// </summary>
    [Table("ExportLog", Schema = "dbo")]
    public class ExportLog
    {
        /// <summary>
        /// Gets or sets the unique identifier (export ID).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExportID { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [MaxLength(20)]
        public string? Status { get; set; }
    }
}

