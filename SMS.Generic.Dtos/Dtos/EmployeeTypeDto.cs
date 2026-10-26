using Newtonsoft.Json;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmployeeType operations.
    /// </summary>
    public class EmployeeTypeDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeDto"/> class.
        /// </summary>
        public EmployeeTypeDto() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeTypeDto"/> class with the specified values.
        /// </summary>
        public EmployeeTypeDto(
            Guid employeeTypeId,
            Guid departmentId,
            string employeeTypeShort,
            string employeeTypeLong,
            bool employeeTypeActive,
            int rank,
            bool isSystemUser,
            bool isSupervisor = false,
            bool isEligible = false)
        {
            EmployeeTypeID = employeeTypeId;
            DepartmentID = departmentId;
            EmployeeTypeShort = employeeTypeShort;
            EmployeeTypeLong = employeeTypeLong;
            EmployeeTypeActive = employeeTypeActive;
            Rank = rank;
            IsSystemUser = isSystemUser;
            IsSupervisor = isSupervisor;
            IsEligible = isEligible;
        }

        /// <summary>
        /// Gets or sets the unique identifier for the employee type.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("employeeTypeID")]
        [System.Text.Json.Serialization.JsonPropertyName("employeeTypeID")]
        public Guid EmployeeTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (auto-populated from token, not required in request).
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the employee type short name.
        /// </summary>
        public string? EmployeeTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the employee type long name.
        /// </summary>
        public string? EmployeeTypeLong { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is active.
        /// </summary>
        public bool EmployeeTypeActive { get; set; }

        /// <summary>
        /// Gets or sets the pay rate.
        /// </summary>
        public decimal? PayRate { get; set; }

        /// <summary>
        /// Gets or sets the premium pay rate.
        /// </summary>
        public decimal? PremiumPayRate { get; set; }

        /// <summary>
        /// Gets or sets the holiday pay rate.
        /// </summary>
        public decimal? HolidayPayRate { get; set; }

        /// <summary>
        /// Gets or sets the rank (0 = no rank/unassigned).
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is for system users.
        /// </summary>
        public bool IsSystemUser { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is a supervisor role.
        /// </summary>
        public bool IsSupervisor { get; set; }

        /// <summary>
        /// Gets or sets whether the employee type is eligible.
        /// </summary>
        public bool IsEligible { get; set; }


        /// <summary>
        /// Gets or sets the list of employee type IDs considered the same rank (for eligibility/assignment).
        /// </summary>
        public List<Guid>? SameRankIds { get; set; }

        public int GetEmployeeRank()
        {
            return Rank;
        }

        /// <summary>
        /// Formats job rank from <paramref name="employeeTypeShort"/>:
        /// single word -> full word, multiple words -> first character of every word.
        /// Examples: "OFC" -> "OFC", "Chief of Police" -> "COP".
        /// </summary>
        public static string? FormatJobRankFromEmployeeTypeShort(string? employeeTypeShort)
        {
            if (string.IsNullOrWhiteSpace(employeeTypeShort))
                return null;

            var value = employeeTypeShort.Trim();
            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length <= 1)
                return value;

            return string.Concat(words.Select(w => char.ToUpperInvariant(w[0])));
        }
    }
}
