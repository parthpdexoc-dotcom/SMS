using SMS.Generic.Dtos.Interfaces;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// DTO for EmployeeTenure operations.
    /// </summary>
    public class EmployeeTenureDto : BaseDto
    {
        public EmployeeTenureDto()
        {
        }

        public EmployeeTenureDto(
            Guid? employeeTenureID,
            Guid? departmentID,
            Guid? employeeID,
            Guid? employeeTypeID,
            DateOnly? dateOfHire,
            DateOnly? dateOfPromotion,
            bool isCurrent)
        {
            EmployeeTenureID = employeeTenureID;
            DepartmentID = departmentID;
            EmployeeID = employeeID;
            EmployeeTypeID = employeeTypeID;
            DateOfHire = dateOfHire;
            DateOfPromotion = dateOfPromotion;
            IsCurrent = isCurrent;
        }

        [Newtonsoft.Json.JsonProperty("employeeTenureID")]
        [System.Text.Json.Serialization.JsonPropertyName("employeeTenureID")]
        public Guid? EmployeeTenureID { get; set; }

        [Newtonsoft.Json.JsonProperty("departmentID")]
        [System.Text.Json.Serialization.JsonPropertyName("departmentID")]
        public Guid? DepartmentID { get; set; }

        [Newtonsoft.Json.JsonProperty("employeeID")]
        [System.Text.Json.Serialization.JsonPropertyName("employeeID")]
        public Guid? EmployeeID { get; set; }

        [Newtonsoft.Json.JsonProperty("employeeTypeID")]
        [System.Text.Json.Serialization.JsonPropertyName("employeeTypeID")]
        public Guid? EmployeeTypeID { get; set; }

        [Newtonsoft.Json.JsonProperty("dateOfHire")]
        [System.Text.Json.Serialization.JsonPropertyName("dateOfHire")]
        public DateOnly? DateOfHire { get; set; }

        [Newtonsoft.Json.JsonProperty("dateOfPromotion")]
        [System.Text.Json.Serialization.JsonPropertyName("dateOfPromotion")]
        public DateOnly? DateOfPromotion { get; set; }

        [Newtonsoft.Json.JsonProperty("isCurrent")]
        [System.Text.Json.Serialization.JsonPropertyName("isCurrent")]
        public bool IsCurrent { get; set; }

        public int Rank { get; set; }

        public string? EmployeeTypeShort { get; set; }
    }
}


