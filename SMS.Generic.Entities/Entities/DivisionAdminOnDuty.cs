using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a division admin on duty entity in the system.
    /// </summary>
    [Table("DivisionAdminOnDuty", Schema = "dbo")]
    public class DivisionAdminOnDuty
    {
        /// <summary>
        /// Gets or sets the unique identifier for the division admin on duty.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

