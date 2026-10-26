using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for HrRequestClass operations.
    /// </summary>
    public class HrRequestClassDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR request class.
        /// </summary>
        public int HrRequestClassID { get; set; }

        /// <summary>
        /// Gets or sets the HR request class short name.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HrRequestClassShort cannot exceed 100 characters")]
        public string? HrRequestClassShort { get; set; }

        /// <summary>
        /// Gets or sets the HR request class long name.
        /// </summary>
        [MaxLength(100, ErrorMessage = "HrRequestClassLong cannot exceed 100 characters")]
        public string? HrRequestClassLong { get; set; }

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

