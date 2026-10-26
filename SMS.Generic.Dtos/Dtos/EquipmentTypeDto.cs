using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EquipmentType operations.
    /// </summary>
    public class EquipmentTypeDto: BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the equipment type.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("equipementTypeID")]
        [System.Text.Json.Serialization.JsonPropertyName("equipementTypeID")]
        public Guid? EquipementTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the equipment type short name.
        /// </summary>
        public string? EquipementTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the equipment type long name.
        /// </summary>
        public string? EquipementTypeLong { get; set; }

        /// <summary>
        /// Gets or sets whether this equipment type is a vehicle.
        /// </summary>
        public bool? IsVehicle { get; set; }

        /// <summary>
        /// Gets or sets the equipment type status.
        /// </summary>
        public bool? EquipementTypeStatus { get; set; }

    }
}


