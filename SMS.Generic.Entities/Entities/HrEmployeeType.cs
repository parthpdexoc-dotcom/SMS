using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an HR employee type entity in the system.
    /// </summary>
    [Table("HrEmployeeType", Schema = "dbo")]
    public class HrEmployeeType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR employee type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HrEmpTypeID { get; set; }

        /// <summary>
        /// Gets or sets the short name for the HR employee type.
        /// </summary>
        [MaxLength(100)]
        public string? HrEmpTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the long name for the HR employee type.
        /// </summary>
        [MaxLength(100)]
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

