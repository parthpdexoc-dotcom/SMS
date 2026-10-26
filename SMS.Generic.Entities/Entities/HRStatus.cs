using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an HR status entity in the system.
    /// </summary>
    [Table("HRStatus", Schema = "dbo")]
    public class HRStatus
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR status.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HRStatusID { get; set; }

        /// <summary>
        /// Gets or sets the short HR status description.
        /// </summary>
        [MaxLength(100)]
        public string? HRStatusShort { get; set; }

        /// <summary>
        /// Gets or sets the long HR status description.
        /// </summary>
        [MaxLength(100)]
        public string? HRStatusLong { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was added.
        /// </summary>
        public DateTime? DtsAdded { get; set; }

        /// <summary>
        /// Gets or sets the admin ID who added the record.
        /// </summary>
        public int? AddedByAdminID { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last updated the record.
        /// </summary>
        public int? UpdateBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was last updated.
        /// </summary>
        public DateTime? UpdatedDatetime { get; set; }

        /// <summary>
        /// Gets or sets the user ID who deleted the record.
        /// </summary>
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was deleted.
        /// </summary>
        public DateTime? DeletedDatetime { get; set; }
    }
}

