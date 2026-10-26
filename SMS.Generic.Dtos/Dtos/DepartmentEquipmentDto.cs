using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// DTO for DepartmentEquipment operations.
    /// </summary>
    public class DepartmentEquipmentDto : BaseDto, IDepartmentScoped
    {
        [Newtonsoft.Json.JsonProperty("departmentEquipmentID")]
        [System.Text.Json.Serialization.JsonPropertyName("departmentEquipmentID")]
        public Guid? DepartmentEquipmentID { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        public Guid EquipmentTypeID { get; set; }

        public int? ChargedType { get; set; }

        public decimal? Charge { get; set; }

        public bool? ChargeMileage { get; set; }

        public decimal? MileageCharge { get; set; }

        public int? MileageCondition { get; set; }

        public bool? EquipmentStatus { get; set; }

        public bool? RequiresApproval { get; set; }

        /// <summary>
        /// List of VehicleApprovalRouting DTOs for vehicle approval routing.
        /// </summary>
        public List<VehicleApprovalRoutingDto>? VehicleApprovalRoutings { get; set; }

        public bool? RequiresUtilizationRecorded { get; set; }
        public bool IsDefault { get; set; }
        public EquipmentType? EquipmentType { get; set; }

    }
    public class DepartmentEquipmentResponseDto : DepartmentEquipmentDto
    {
        public string? EquipmentType { get; set; }
    }
}


