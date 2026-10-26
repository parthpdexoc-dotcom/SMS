using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentActivityType operations.
    /// </summary>
    public class DepartmentActivityTypeDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Primary Key (GUID) - Unique identifier for the activity type.
        /// </summary>
        public Guid DepartmentActivityTypeId { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Short name or code for the activity type.
        /// </summary>
        public string ActivityTypeShort { get; set; } = string.Empty;

        /// <summary>
        /// Long description for the activity type.
        /// </summary>
        public string ActivityTypeLong { get; set; } = string.Empty;

        /// <summary>
        /// Status of the activity type (active/inactive).
        /// </summary>
        public bool ActivityTypeStatus { get; set; }

    }
}
