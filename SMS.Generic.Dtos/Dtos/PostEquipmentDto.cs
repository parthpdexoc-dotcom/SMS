using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PostEquipment operations.
    /// </summary>
    public class PostEquipmentDto : BaseDto
    {
        /// <summary>
        /// Primary Key - Post Equipment ID (GUID)
        /// </summary>
        public Guid? PostEquipmentID { get; set; }

        /// <summary>
        /// Foreign Key - Post ID
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// Foreign Key - Department Equipment ID
        /// </summary>
        public Guid? DepartmentEquipmentID { get; set; }

        /// <summary>
        /// When true, Per Job Position (every filled position, quantity 1). When false, Per Job (rank then hire date, up to quantity).
        /// </summary>
        public bool EquipmentPerPostPosition { get; set; }

        /// <summary>
        /// Equipment quantity
        /// </summary>
        public int EquipmentQuantity { get; set; }

        /// <summary>
        /// Indicates whether the equipment is a vehicle (true/false)
        /// </summary>
        public bool IsVehicle { get; set; }

        /// <summary>
        /// Equipment effective start date for this post assignment.
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Equipment effective end date for this post assignment.
        /// </summary>
        public DateOnly EndDate { get; set; }

        public PostEquipmentDto CloneForPost(Guid newPostId)
        {
            return new PostEquipmentDto
            {
                PostEquipmentID = Guid.Empty,
                PostID = newPostId,
                DepartmentEquipmentID = DepartmentEquipmentID,
                EquipmentPerPostPosition = EquipmentPerPostPosition,
                EquipmentQuantity = EquipmentQuantity,
                IsVehicle = IsVehicle,
                StartDate = StartDate,
                EndDate = EndDate
            };
        }
    }
}
