using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeDeviceToken entity.
    /// </summary>
    public class EmployeeDeviceTokenDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the employee device token record.
        /// </summary>
        public Guid? EmployeeDeviceTokenID { get; set; }

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
    }
}
