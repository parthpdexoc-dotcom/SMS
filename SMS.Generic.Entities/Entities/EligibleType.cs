using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an eligible type entity in the system.
    /// </summary>
    [Table("EligibleType", Schema = "dbo")]
    public class EligibleType
    {
        /// <summary>
        /// Gets or sets the unique identifier for the eligible type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EligibleCID { get; set; }

        /// <summary>
        /// Gets or sets the eligible department identifier.
        /// </summary>
        public int? EligibleDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the eligible type identifier.
        /// </summary>
        public int? EligibleTypeID { get; set; }

        /// <summary>
        /// Gets or sets the eligible title.
        /// </summary>
        public string? EligibleTitle { get; set; }

        /// <summary>
        /// Gets or sets the eligible value.
        /// </summary>
        public int? EligibleValue { get; set; }

        /// <summary>
        /// Gets or sets the eligible range from value.
        /// </summary>
        public int? EligibleRangeFrom { get; set; }

        /// <summary>
        /// Gets or sets the eligible range to value.
        /// </summary>
        public int? EligibleRangeTO { get; set; }

        /// <summary>
        /// Gets or sets whether the eligible type is active.
        /// </summary>
        public bool? IsActive { get; set; }
    }
}

