using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a vehicle entity in the system.
    /// </summary>
    [Table("Vehicle", Schema = "dbo")]
    public class Vehicle
    {
        /// <summary>
        /// Gets or sets the unique identifier for the vehicle.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VehicleID { get; set; }

        /// <summary>
        /// Gets or sets the vehicle number.
        /// </summary>
        [MaxLength(20)]
        public string? VehicleNo { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(75)]
        public string? Descrption { get; set; }

        /// <summary>
        /// Gets or sets the vehicle type.
        /// </summary>
        [MaxLength(20)]
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

