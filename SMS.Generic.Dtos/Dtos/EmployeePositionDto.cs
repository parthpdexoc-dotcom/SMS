using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeePosition operations.
    /// </summary>
    public class EmployeePositionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee position.
        /// </summary>
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
        public string? Title { get; set; }
    }
}

