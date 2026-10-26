using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for BlockPostDay operations.
    /// </summary>
    public class BlockPostDayDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the block post day.
        /// </summary>
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

