using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for HRStatus operations.
    /// </summary>
    public class HRStatusDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR status.
        /// </summary>
        public int HRStatusID { get; set; }

        /// <summary>
        /// Gets or sets the short HR status description.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HRStatusShort cannot exceed 100 characters")]
        public string? HRStatusShort { get; set; }

        /// <summary>
        /// Gets or sets the long HR status description.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HRStatusLong cannot exceed 100 characters")]
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

