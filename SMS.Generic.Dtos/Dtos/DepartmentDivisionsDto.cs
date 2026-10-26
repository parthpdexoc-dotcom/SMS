using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentDivisions operations.
    /// </summary>
    public class DepartmentDivisionsDto: BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDivisionsDto"/> class.
        /// </summary>
        public DepartmentDivisionsDto() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDivisionsDto"/> class with the specified values.
        /// </summary>
        public DepartmentDivisionsDto(Guid? divisionId, Guid departmentId, string divisionShort, string divisionLong, bool divisionActive)
        {
            DivisionID = divisionId;
            DepartmentID = departmentId;
            DivisionShort = divisionShort;
            DivisionLong = divisionLong;
            DivisionActive = divisionActive;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the division.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("divisionID")]
        [System.Text.Json.Serialization.JsonPropertyName("divisionID")]
        public Guid? DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the department ID that this division belongs to.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the division.
        /// </summary>
        public string DivisionShort { get; set; }

        /// <summary>
        /// Gets or sets whether the division is active.
        /// </summary>
        public bool DivisionActive { get; set; }

        /// <summary>
        /// Gets or sets the long name or description of the division.
        /// </summary>
        public string DivisionLong { get; set; }

    }
}

