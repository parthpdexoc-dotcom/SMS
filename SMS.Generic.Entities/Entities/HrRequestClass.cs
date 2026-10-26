using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an HR request class entity in the system.
    /// </summary>
    [Table("HrRequestClass", Schema = "dbo")]
    public class HrRequestClass
    {
        /// <summary>
        /// Gets or sets the unique identifier for the HR request class.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HrRequestClassID { get; set; }

        /// <summary>
        /// Gets or sets the HR request class short name.
        /// </summary>
        [MaxLength(100)]
        public string? HrRequestClassShort { get; set; }

        /// <summary>
        /// Gets or sets the HR request class long name.
        /// </summary>
        [MaxLength(100)]
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

