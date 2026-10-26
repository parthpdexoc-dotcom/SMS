using Newtonsoft.Json;
using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentResponseArea operations.
    /// </summary>
    public class DepartmentResponseAreaDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the response area.
        /// Optional: If not provided or empty, a new record will be created.
        /// If provided, the record will be updated if it exists, otherwise returns not found.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("responseAreaID")]
        [System.Text.Json.Serialization.JsonPropertyName("responseAreaID")]
        public Guid? DepartmentResponseAreaID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the response area.
        /// </summary>
        [Required(ErrorMessage = "DepartmentResponseAreaShort is required")]
        [MaxLength(50, ErrorMessage = "DepartmentResponseAreaShort cannot exceed 50 characters")]
        public string DepartmentResponseAreaShort { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the long description of the response area.
        /// </summary>
        [Required(ErrorMessage = "DepartmentResponseAreaLong is required")]
        [MaxLength(200, ErrorMessage = "DepartmentResponseAreaLong cannot exceed 200 characters")]
        public string DepartmentResponseAreaLong { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status of the response area (true or false).
        /// </summary>
        [Required(ErrorMessage = "DepartmentResponseAreaStatus is required")]
        public bool DepartmentResponseAreaStatus { get; set; }

    }
}
