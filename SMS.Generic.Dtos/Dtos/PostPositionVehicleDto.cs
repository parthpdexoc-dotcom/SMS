namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PostPositionVehicle operations.
    /// </summary>
    public class PostPositionVehicleDto : BaseDto
    {
        /// <summary>
        /// Primary Key - Post Position Vehicle ID (GUID).
        /// </summary>
        public Guid? PostPositionVehicleID { get; set; }

        /// <summary>
        /// Foreign Key - Post Position ID.
        /// </summary>
        public Guid? PostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID.
        /// </summary>
        public Guid? DepartmentEquipmentID { get; set; }

        /// <summary>
        /// Charged type (per hour / per item).
        /// </summary>
        public int? ChargedType { get; set; }

        /// <summary>
        /// Base charge amount.
        /// </summary>
        public decimal? Charge { get; set; }

        /// <summary>
        /// Whether mileage is charged.
        /// </summary>
        public bool? ChargeMileage { get; set; }

        /// <summary>
        /// Money value per mile.
        /// </summary>
        public decimal? MileageCharge { get; set; }

        /// <summary>
        /// Mileage condition.
        /// </summary>
        public int? MileageCondition { get; set; }

        /// <summary>
        /// Equipment quantity assigned (frozen at shift freeze / start).
        /// </summary>
        public int? EquipmentQuantity { get; set; }

        /// <summary>
        /// Job type frozen at shift freeze / start.
        /// When true, Per Position; when false, Per Job / Per Shift.
        /// Same semantics as <see cref="PostEquipmentDto.EquipmentPerPostPosition"/>.
        /// </summary>
        public bool EquipmentPerPostPosition { get; set; }
    }
}
