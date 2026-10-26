using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DivisionAdminOnDuty operations.
    /// </summary>
    public class DivisionAdminOnDutyDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the division admin on duty.
        /// </summary>
        public int DivisionAdminOnDutyID { get; set; }

        /// <summary>
        /// Gets or sets the division identifier.
        /// </summary>
        public int? DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        public int? DepartmentID { get; set; }
    }
}

