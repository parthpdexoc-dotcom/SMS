using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for BusinessType operations.
    /// </summary>
    public class BusinessTypeDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the business type.
        /// Optional: If not provided or empty, a new record will be created.
        /// If provided, the record will be updated if it exists, otherwise returns not found.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("businessTypeID")]
        [System.Text.Json.Serialization.JsonPropertyName("businessTypeID")]
        public Guid? BusinessTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the business type.
        /// </summary>
        public string BusinessTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the long description of the business type.
        /// </summary>
        public string BusinessTypeLong { get; set; }

        /// <summary>
        /// Gets or sets the status of the business type.
        /// </summary>
        public bool BusinessTypeStatus { get; set; }

    }
}

