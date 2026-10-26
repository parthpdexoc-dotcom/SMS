using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Position operations.
    /// </summary>
    public class PositionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the position.
        /// </summary>
        public long PositionID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the post ID.
        /// </summary>
        public long? PostID { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        [MaxLength(20, ErrorMessage = "StartTime cannot exceed 20 characters")]
        public string? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the hours worked.
        /// </summary>
        public float? HoursWorked { get; set; }

        /// <summary>
        /// Gets or sets the post day.
        /// </summary>
        public DateTime? PostDay { get; set; }

        /// <summary>
        /// Gets or sets the call sign.
        /// </summary>
        [MaxLength(50, ErrorMessage = "CallSign cannot exceed 50 characters")]
        public string? CallSign { get; set; }

        /// <summary>
        /// Gets or sets whether the position is canceled.
        /// </summary>
        public bool? Canceled { get; set; }

        /// <summary>
        /// Gets or sets the archive status.
        /// </summary>
        public short? Archieve { get; set; }

        /// <summary>
        /// Gets or sets the upload date.
        /// </summary>
        public DateTime? UploadDate { get; set; }

        /// <summary>
        /// Gets or sets whether the position is confirmed.
        /// </summary>
        public bool? Confirm { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets the vehicle number.
        /// </summary>
        [MaxLength(15, ErrorMessage = "VNumber cannot exceed 15 characters")]
        public string? VNumber { get; set; }

        /// <summary>
        /// Gets or sets the car hours.
        /// </summary>
        public int? CarHours { get; set; }

        /// <summary>
        /// Gets or sets the mileage.
        /// </summary>
        public float? Mileage { get; set; }

        /// <summary>
        /// Gets or sets whether the car is city owned.
        /// </summary>
        public bool? CityOwned { get; set; }

        /// <summary>
        /// Gets or sets whether the car is reimbursed.
        /// </summary>
        public bool? CarReimbursed { get; set; }

        /// <summary>
        /// Gets or sets the division ID.
        /// </summary>
        public int? DivisionID { get; set; }

        /// <summary>
        /// Gets or sets whether worked on duty.
        /// </summary>
        public bool? WorkedOnDuty { get; set; }

        /// <summary>
        /// Gets or sets the seniority status.
        /// </summary>
        public bool? Seniority { get; set; }

        /// <summary>
        /// Gets or sets the reimburse status.
        /// </summary>
        public byte? ReimburseStatus { get; set; }

        /// <summary>
        /// Gets or sets the pay rate.
        /// </summary>
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Gets or sets the reimbursed pay rate.
        /// </summary>
        public decimal? ReimbursedPayRate { get; set; }

        /// <summary>
        /// Gets or sets who closed the position.
        /// </summary>
        [MaxLength(40, ErrorMessage = "ClosedBy cannot exceed 40 characters")]
        public string? ClosedBy { get; set; }

        /// <summary>
        /// Gets or sets when the position was closed.
        /// </summary>
        public DateTime? ClosedWhen { get; set; }

        /// <summary>
        /// Gets or sets whether the position is split.
        /// </summary>
        public bool? Split { get; set; }

        /// <summary>
        /// Gets or sets the activity status.
        /// </summary>
        public bool? Activity { get; set; }

        /// <summary>
        /// Gets or sets the compensation status.
        /// </summary>
        public bool? Compensation { get; set; }

        /// <summary>
        /// Gets or sets whether canceled by employee.
        /// </summary>
        public bool? CanceledByEmployee { get; set; }

        /// <summary>
        /// Gets or sets when canceled by employee.
        /// </summary>
        public DateTime? CanceledByEmployeeTime { get; set; }

        /// <summary>
        /// Gets or sets the canceled release date.
        /// </summary>
        public DateTime? CanceledReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets whether the position is private.
        /// </summary>
        public bool? Private { get; set; }

        /// <summary>
        /// Gets or sets the hours required.
        /// </summary>
        public int? HoursRequired { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        [MaxLength(16, ErrorMessage = "IP cannot exceed 16 characters")]
        public string? IP { get; set; }

        /// <summary>
        /// Gets or sets the allowed positions.
        /// </summary>
        public short? Allowed { get; set; }

        /// <summary>
        /// Gets or sets the positions count.
        /// </summary>
        public short? Positions { get; set; }

        /// <summary>
        /// Gets or sets whether passed to payroll.
        /// </summary>
        public bool? PassedToPayroll { get; set; }

        /// <summary>
        /// Gets or sets the real hours required.
        /// </summary>
        public float? RealHoursRequired { get; set; }

        /// <summary>
        /// Gets or sets the RSTDTS2 status.
        /// </summary>
        public bool? RSTDTS2 { get; set; }

        /// <summary>
        /// Gets or sets whether PR is applied.
        /// </summary>
        public bool? PRApplied { get; set; }

        /// <summary>
        /// Gets or sets the minimum hours paid.
        /// </summary>
        public float? MinHrsPaid { get; set; }

        /// <summary>
        /// Gets or sets whether minimum is due.
        /// </summary>
        public bool? IsMinDue { get; set; }

        /// <summary>
        /// Gets or sets whether payment is done.
        /// </summary>
        public bool? IsPaymentDone { get; set; }

        /// <summary>
        /// Gets or sets the payment track information.
        /// </summary>
        public string? PaymentTrack { get; set; }
    }
}

