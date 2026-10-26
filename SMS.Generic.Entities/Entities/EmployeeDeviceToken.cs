using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an employee's device token for push notifications.
    /// </summary>
    [Table("EmployeeDeviceToken", Schema = "dbo")]
    public class EmployeeDeviceToken
    {
        /// <summary>
        /// Parameterless constructor for EF Core.
        /// </summary>
        public EmployeeDeviceToken() { }

        /// <summary>
        /// Creates an employee device token with the given employee and token.
        /// </summary>
        public EmployeeDeviceToken(Guid employeeId, string deviceToken)
        {
            EmployeeID = employeeId;
            DeviceToken = deviceToken;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the employee device token record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeDeviceTokenID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier (foreign key to Employee).
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the device token (e.g. for FCM, APNs).
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string DeviceToken { get; set; } = null!;

        /// <summary>
        /// Navigation property to the employee.
        /// </summary>
        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }
    }
}
