using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeStatus operations.
    /// </summary>
    public class EmployeeStatusDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeStatusDto"/> class.
        /// </summary>
        public EmployeeStatusDto() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeStatusDto"/> class with the specified values.
        /// </summary>
        public EmployeeStatusDto(Guid employeeStatusId, Guid departmentId, string statusShort, string statusLong, bool status)
        {
            EmployeeStatusID = employeeStatusId;
            DepartmentID = departmentId;
            StatusShort = statusShort;
            StatusLong = statusLong;
            Status = status;
        }

        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("employeeStatusID")]
        [System.Text.Json.Serialization.JsonPropertyName("employeeStatusID")]
        public Guid EmployeeStatusID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Short label for employee status.
        /// </summary>
        public string? StatusShort { get; set; }

        /// <summary>
        /// Long description for employee status.
        /// </summary>
        public string? StatusLong { get; set; }

        /// <summary>
        /// Status 
        /// </summary>
        public bool Status { get; set; }

    }
}

