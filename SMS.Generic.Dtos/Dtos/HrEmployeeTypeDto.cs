using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for HrEmployeeType operations.
    /// </summary>
    public class HrEmployeeTypeDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR employee type.
        /// </summary>
        public int HrEmpTypeID { get; set; }

        /// <summary>
        /// Gets or sets the short name for the HR employee type.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HrEmpTypeShort cannot exceed 100 characters")]
        public string? HrEmpTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the long name for the HR employee type.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HrEmpTypeLong cannot exceed 100 characters")]
        public string? HrEmpTypeLong { get; set; }

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

