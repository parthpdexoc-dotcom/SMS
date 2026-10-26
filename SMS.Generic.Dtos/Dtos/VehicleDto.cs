using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Vehicle operations.
    /// </summary>
    public class VehicleDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the vehicle.
        /// </summary>
        public int VehicleID { get; set; }

        /// <summary>
        /// Gets or sets the vehicle number.
        /// </summary>
        [MaxLength(20, ErrorMessage = "VehicleNo cannot exceed 20 characters")]
        public string? VehicleNo { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(75, ErrorMessage = "Descrption cannot exceed 75 characters")]
        public string? Descrption { get; set; }

        /// <summary>
        /// Gets or sets the vehicle type.
        /// </summary>
        [MaxLength(20, ErrorMessage = "VehicleType cannot exceed 20 characters")]
        public string? VehicleType { get; set; }

        /// <summary>
        /// Gets or sets the division ID.
        /// </summary>
        public int? DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public int? DepartmentID { get; set; }
    }
}

