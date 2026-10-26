using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an employee position entity in the system.
    /// </summary>
    [Table("EmployeePosition", Schema = "dbo")]
    public class EmployeePosition
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee position.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeePositionID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the position number.
        /// </summary>
        public int? PositionNo { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [MaxLength(50)]
        public string? Title { get; set; }
    }
}

