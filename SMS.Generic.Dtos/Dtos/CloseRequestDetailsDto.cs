using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for CloseRequestDetails operations.
    /// </summary>
    public class CloseRequestDetailsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the close request detail.
        /// </summary>
        public long CloseRequestDetailID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        [Required(ErrorMessage = "DepartmentID is required")]
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the position ID.
        /// </summary>
        [Required(ErrorMessage = "PositionID is required")]
        public long PositionID { get; set; }

        /// <summary>
        /// Gets or sets the post day.
        /// </summary>
        [Required(ErrorMessage = "PostDay is required")]
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
        [Required(ErrorMessage = "IsWorked is required")]
        public bool IsWorked { get; set; }

        /// <summary>
        /// Gets or sets the reason for no work.
        /// </summary>
        [MaxLength(10, ErrorMessage = "NoWorkReason cannot exceed 10 characters")]
        public string? NoWorkReason { get; set; }

        /// <summary>
        /// Gets or sets whether there is unpaid break time.
        /// </summary>
        public bool? IsUnPaidBreakTime { get; set; }

        /// <summary>
        /// Gets or sets the break time in hours.
        /// </summary>
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
        public decimal? BgnMilage { get; set; }

        /// <summary>
        /// Gets or sets the ending mileage.
        /// </summary>
        public decimal? EndMilage { get; set; }

        /// <summary>
        /// Gets or sets who closed the request.
        /// </summary>
        [MaxLength(50, ErrorMessage = "CloseBy cannot exceed 50 characters")]
        public string? CloseBy { get; set; }

        /// <summary>
        /// Gets or sets when the request was closed.
        /// </summary>
        public DateTime? CloseWhen { get; set; }

        /// <summary>
        /// Gets or sets the minimum hours paid.
        /// </summary>
        public float? MinimumHoursPaid { get; set; }

        /// <summary>
        /// Gets or sets the total hours paid.
        /// </summary>
        public float? TotalHoursPaid { get; set; }
    }
}

