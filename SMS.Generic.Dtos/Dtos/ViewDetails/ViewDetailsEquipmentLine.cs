namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Request or post equipment line for View Details equipment card.</summary>
    public class ViewDetailsEquipmentLine
    {
        public ViewDetailsEquipmentLine()
        {
        }

        public ViewDetailsEquipmentLine(
            Guid? requestEquipmentId,
            Guid? postEquipmentId,
            Guid? postId,
            Guid? departmentEquipmentId,
            bool equipmentPerPost,
            bool equipmentPerPostPosition,
            int equipmentQuantity,
            bool isVehicle,
            string? equipmentTypeName,
            decimal? charge,
            decimal? mileageCharge,
            bool? chargeMileage,
            bool? requiresApproval,
            int? chargeType)
        {
            RequestEquipmentId = requestEquipmentId;
            PostEquipmentId = postEquipmentId;
            PostId = postId;
            DepartmentEquipmentId = departmentEquipmentId;
            EquipmentPerPost = equipmentPerPost;
            EquipmentPerPostPosition = equipmentPerPostPosition;
            EquipmentQuantity = equipmentQuantity;
            IsVehicle = isVehicle;
            EquipmentTypeName = equipmentTypeName;
            Charge = charge;
            MileageCharge = mileageCharge;
            ChargeMileage = chargeMileage;
            RequiresApproval = requiresApproval;
            ChargeType = chargeType;
        }

        public Guid? RequestEquipmentId { get; set; }
        public Guid? PostEquipmentId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? DepartmentEquipmentId { get; set; }
        /// <summary>When set, request-level equipment scoped by <see cref="EquipmentPerPost"/>.</summary>
        public bool EquipmentPerPost { get; set; }
        /// <summary>When set, post-level equipment scoped by <see cref="EquipmentPerPostPosition"/>.</summary>
        public bool EquipmentPerPostPosition { get; set; }
        public int EquipmentQuantity { get; set; }
        public bool IsVehicle { get; set; }
        /// <summary>Short or long equipment type label when available.</summary>
        public string? EquipmentTypeName { get; set; }

        /// <summary>Aligned with <see cref="DepartmentEquipmentDto.Charge"/> (department equipment card).</summary>
        public decimal? Charge { get; set; }
        public decimal? MileageCharge { get; set; }
        public bool? ChargeMileage { get; set; }
        public bool? RequiresApproval { get; set; }
        /// <summary>Aligned with <see cref="DepartmentEquipmentDto.ChargedType"/> (department equipment card).</summary>
        public int? ChargeType { get; set; }
    }
}
