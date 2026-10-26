using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Equipment operations.
    /// </summary>
    public class EquipmentDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the equipment.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("equipementID")]
        [System.Text.Json.Serialization.JsonPropertyName("equipementID")]
        public Guid? EquipementID { get; set; }

        /// <summary>
        /// Gets or sets the department ID that this equipment belongs to.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the equipment type ID.
        /// </summary>
        [Required(ErrorMessage = "EquipementTypeID is required")]
        public Guid EquipementTypeID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the equipment.
        /// </summary>
        [Required(ErrorMessage = "EquipementShort is required")]
        [MaxLength(50, ErrorMessage = "EquipementShort cannot exceed 50 characters")]
        public string? EquipementShort { get; set; }

        /// <summary>
        /// Gets or sets the long name or description of the equipment.
        /// </summary>
        [Required(ErrorMessage = "EquipementLong is required")]
        [MaxLength(200, ErrorMessage = "EquipementLong cannot exceed 200 characters")]
        public string? EquipementLong { get; set; }

        /// <summary>
        /// Gets or sets the charged type (per hour/per item).
        /// </summary>
        [Required(ErrorMessage = "ChargedType is required")]
        public int? ChargedType { get; set; }

        /// <summary>
        /// Gets or sets whether mileage is charged (will be hidden for vehicle).
        /// </summary>
        [Required(ErrorMessage = "ChargeMileage is required")]
        public bool? ChargeMileage { get; set; }

        /// <summary>
        /// Gets or sets the mileage charge amount (money value per mile).
        /// </summary>
        [Required(ErrorMessage = "MileageCharge is required")]
        public decimal? MileageCharge { get; set; }

        /// <summary>
        /// Gets or sets whether the equipment is reimbursable.
        /// </summary>
        [Required(ErrorMessage = "IsReimburseable is required")]
        public bool? IsReimburseable { get; set; }

        /// <summary>
        /// Gets or sets who to reimburse to (city/officer).
        /// </summary>
        [Required(ErrorMessage = "ReimburseTo is required")]
        public int? ReimburseTo { get; set; }

        /// <summary>
        /// Gets or sets the equipment status.
        /// </summary>
        [Required(ErrorMessage = "EquipementStatus is required")]
        public bool? EquipementStatus { get; set; }
    }
}

