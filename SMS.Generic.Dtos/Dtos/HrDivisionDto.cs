using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for HrDivision operations.
    /// </summary>
    public class HrDivisionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR division.
        /// </summary>
        public int HrDivisionID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the HR division.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HrDivisionShort cannot exceed 100 characters")]
        public string? HrDivisionShort { get; set; }

        /// <summary>
        /// Gets or sets the long name of the HR division.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HrDivisionLong cannot exceed 100 characters")]
        public string? HrDivisionLong { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the division was added.
        /// </summary>
        public DateTime? DtsAdded { get; set; }

        /// <summary>
        /// Gets or sets the admin ID who added the division.
        /// </summary>
        public int? AddedByAdminID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who last updated the division.
        /// </summary>
        public int? UpdateBy { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the division was last updated.
        /// </summary>
        public DateTime? UpdatedDatetime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who deleted the division.
        /// </summary>
        public int? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the date/time when the division was deleted.
        /// </summary>
        public DateTime? DeletedDatetime { get; set; }
    }
}

