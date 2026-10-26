using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a close request detail entity in the system.
    /// </summary>
    [Table("CloseRequestDetails", Schema = "dbo")]
    public class CloseRequestDetails
    {
        /// <summary>
        /// Gets or sets the unique identifier for the close request detail.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CloseRequestDetailID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        [Required]
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the position ID.
        /// </summary>
        [Required]
        public long PositionID { get; set; }

        /// <summary>
        /// Gets or sets the post day.
        /// </summary>
        [Required]
        public DateTime PostDay { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Gets or sets whether the position was worked.
        /// </summary>
        [Required]
        public bool IsWorked { get; set; }

        /// <summary>
        /// Gets or sets the reason for no work.
        /// </summary>
        [MaxLength(10)]
        public string? NoWorkReason { get; set; }

        /// <summary>
        /// Gets or sets whether there is unpaid break time.
        /// </summary>
        public bool? IsUnPaidBreakTime { get; set; }

        /// <summary>
        /// Gets or sets the break time in hours.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BreakTime { get; set; }

        /// <summary>
        /// Gets or sets whether a vehicle was used.
        /// </summary>
        public bool? IsVehicle { get; set; }

        /// <summary>
        /// Gets or sets the vehicle type ID.
        /// </summary>
        public int? VehicleTypeId { get; set; }

        /// <summary>
        /// Gets or sets whether the vehicle is city owned.
        /// </summary>
        public bool? IsCityOwned { get; set; }

        /// <summary>
        /// Gets or sets the beginning mileage.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? BgnMilage { get; set; }

        /// <summary>
        /// Gets or sets the ending mileage.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? EndMilage { get; set; }

        /// <summary>
        /// Gets or sets who closed the request.
        /// </summary>
        [MaxLength(50)]
        public string? CloseBy { get; set; }

        /// <summary>
        /// Gets or sets when the request was closed.
        /// </summary>
        public DateTime? CloseWhen { get; set; }

        /// <summary>
        /// Gets or sets the minimum hours paid.
        /// </summary>
        [Column(TypeName = "real")]
        public float? MinimumHoursPaid { get; set; }

        /// <summary>
        /// Gets or sets the total hours paid.
        /// </summary>
        [Column(TypeName = "real")]
        public float? TotalHoursPaid { get; set; }
    }
}

