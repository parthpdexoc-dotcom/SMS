using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    public class PlaceholderDto : BaseDto, IDepartmentScoped
    {
        public const string EmployeeNumberPrefix = "9900";
        public const string DefaultEmployeeNumberSuffix = "00000";

        public Guid? PlaceholderID { get; set; }

        [JsonProperty("firstName")]
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonProperty("contactNumber")]
        [JsonPropertyName("contactNumber")]
        public string? ContactNumber { get; set; }

        /// <summary>Stored as entered (suffix only, no <see cref="EmployeeNumberPrefix"/>).</summary>
        [JsonProperty("employeeNumber")]
        [JsonPropertyName("employeeNumber")]
        public string? EmployeeNumber { get; set; }

        public Guid DepartmentID { get; set; }

        public PlaceholderStatus PlaceholderStatus { get; set; } = PlaceholderStatus.Visible;

        public bool IsUniquePerPosition { get; set; }

        public string? GetPlaceHolderName() => FormatDisplayName(LastName, FirstName);

        public static string? FormatDisplayName(Placeholder? placeholder) =>
            placeholder == null ? null : FormatDisplayName(placeholder.LastName, placeholder.FirstName);

        /// <summary>Same display format as <see cref="FormatDisplayName(Placeholder?)"/> for mapped DTOs.</summary>
        public static string? FormatDisplayName(PlaceholderDto? placeholderDetail) =>
            placeholderDetail == null ? null : FormatDisplayName(placeholderDetail.LastName, placeholderDetail.FirstName);

        /// <summary>
        /// Payroll export only: <c>9900</c> + stored number when present; otherwise <c>990000000</c>.
        /// </summary>
        public static string FormatEmployeeNumberForPayroll(string? storedEmployeeNumber)
        {
            if (string.IsNullOrWhiteSpace(storedEmployeeNumber))
                return EmployeeNumberPrefix + DefaultEmployeeNumberSuffix;

            return EmployeeNumberPrefix + storedEmployeeNumber.Trim();
        }
    }

    public class BulkPlaceholderStatusRequestDto
    {
        /// <summary>
        /// Map of placeholder id → desired <see cref="PlaceholderStatus"/>.
        /// </summary>
        public Dictionary<Guid, PlaceholderStatus> Placeholders { get; set; } = new();
    }

    public class BulkPlaceholderStatusItemDto
    {
        public Guid PlaceholderID { get; set; }

        public string? PlaceholderName { get; set; }

        public string? Message { get; set; }

        public bool HasError { get; set; }
    }
}
