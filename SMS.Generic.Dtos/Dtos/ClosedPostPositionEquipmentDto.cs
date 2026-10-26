using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ClosedPostPositionEquipment operations.
    /// </summary>
    public class ClosedPostPositionEquipmentDto : BaseDto
    {
        /// <summary>
        /// Primary Key - Closed Post Position Equipment ID (GUID).
        /// </summary>
        public Guid? ClosedPostPositionEquipmentID { get; set; }

        /// <summary>
        /// Foreign Key - Closed Post Position ID.
        /// </summary>
        public Guid? ClosedPostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID.
        /// </summary>
        public Guid? DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Equipment display name (resolved from DepartmentEquipment.EquipmentType).
        /// Returned on read endpoints for convenience.
        /// </summary>
        public string? DepartmentEquipmentName { get; set; }

        /// <summary>
        /// Whether the selected equipment is a vehicle (resolved from DepartmentEquipment.EquipmentType.IsVehicle).
        /// Returned on read endpoints for convenience.
        /// </summary>
        public bool? DepartmentEquipmentIsVehicle { get; set; }

        /// <summary>
        /// Post ID.
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// Work start date.
        /// </summary>
        public DateTime? WorkStartDate { get; set; }

        /// <summary>
        /// Work start time.
        /// </summary>
        public TimeOnly? WorkStartTime { get; set; }

        /// <summary>
        /// Hours worked.
        /// </summary>
        public int? HoursWorked { get; set; }

        /// <summary>
        /// Indicates whether the position was worked.
        /// </summary>
        public bool? IsWorked { get; set; }

        /// <summary>
        /// Reason when no work was performed.
        /// </summary>
        public string? NoWorkReason { get; set; }

        /// <summary>
        /// Indicates whether break time is unpaid.
        /// </summary>
        public bool? IsUnPaidBreakTime { get; set; }

        /// <summary>
        /// Break time (e.g. in hours).
        /// </summary>
        public decimal? BreakTime { get; set; }

        /// <summary>
        /// Quantity of equipment used.
        /// </summary>
        public int? UsedEquipmentQuantity { get; set; }

        /// <summary>
        /// Hours the equipment was used.
        /// </summary>
        public decimal? EquipmentUsedHours { get; set; }

        /// <summary>
        /// Paid hours.
        /// </summary>
        public decimal? PaidHours { get; set; }

        /// <summary>
        /// Reimbursement recipient (Department / Employee); set server-side from the assigned employee's <see cref="EmployeeDto.IsSubsidizedVehicle"/> flag.
        /// </summary>
        public ReimburseTo? ReimburseTo { get; set; }

        /// <summary>
        /// Pay rate (money).
        /// </summary>
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Reimbursement pay rate (money).
        /// </summary>
        public decimal? ReimbursePayRate { get; set; }

        /// <summary>
        /// Indicates whether the equipment used is a vehicle.
        /// </summary>
        public bool? EquipmentUsedIsVehicle { get; set; }

        /// <summary>
        /// Vehicle ownership (City / Employee / Other); applicable when EquipmentUsedIsVehicle is true.
        /// </summary>
        public VehicleOwnedByType? VehicleOwnedBy { get; set; }

        /// <summary>
        /// Vehicle number.
        /// </summary>
        public string? VehNumber { get; set; }

        /// <summary>
        /// Start mileage; applicable when EquipmentUsedIsVehicle is true.
        /// </summary>
        public decimal? StartMileage { get; set; }

        /// <summary>
        /// End mileage; applicable when EquipmentUsedIsVehicle is true.
        /// </summary>
        public decimal? EndMileage { get; set; }

        /// <summary>
        /// Total mileage (calculated).
        /// </summary>
        public decimal? TotalMileage { get; set; }

        /// <summary>
        /// Vehicle quantity.
        /// </summary>
        public int? VehicleQuantity { get; set; }

        /// <summary>
        /// Vehicle used hours.
        /// </summary>
        public int? VehicleUsedHours { get; set; }
    }
}
