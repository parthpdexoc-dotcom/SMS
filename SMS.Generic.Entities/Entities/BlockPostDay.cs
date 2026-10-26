using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a block post day entity in the system.
    /// </summary>
    [Table("BlockPostDay", Schema = "dbo")]
    public class BlockPostDay
    {
        /// <summary>
        /// Gets or sets the unique identifier for the block post day.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BlockPostID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public int? RequestID { get; set; }

        /// <summary>
        /// Gets or sets the position date.
        /// </summary>
        public DateTime? PositionDate { get; set; }
    }
}

