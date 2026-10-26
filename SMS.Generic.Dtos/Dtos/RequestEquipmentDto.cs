using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequestEquipment operations.
    /// </summary>
    public class RequestEquipmentDto : BaseDto
    {
        /// <summary>
        /// Primary Key - Request Equipment ID (GUID)
        /// </summary>
        public Guid? RequestEquipmentID { get; set; }

        /// <summary>
        /// Foreign Key - Request ID
        /// </summary>
        public Guid? RequestID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID
        /// </summary>
        public Guid? DepartmentEquipmentID { get; set; }

        /// <summary>
        /// When true, Per Job (rank then hire date, up to quantity). When false, Per Job Position (every filled position, quantity 1).
        /// </summary>
        public bool EquipmentPerPost { get; set; }

        /// <summary>
        /// Equipment quantity
        /// </summary>
        public int EquipmentQuantity { get; set; }

        /// <summary>
        /// Indicates whether the equipment is a vehicle (true/false)
        /// </summary>
        public bool IsVehicle { get; set; }

        /// <summary>
        /// Equipment effective start date for this request assignment.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Equipment effective end date for this request assignment.
        /// </summary>
        public DateOnly EndDate { get; set; }
    }
}
