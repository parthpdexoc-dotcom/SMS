using System.ComponentModel.DataAnnotations;
using SMS.Generic.Common.Helpers;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Department operations.
    /// </summary>
    public class DepartmentDto : BaseDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDto"/> class.
        /// </summary>
        public DepartmentDto() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentDto"/> class with the specified values.
        /// </summary>
        public DepartmentDto(Guid departmentId, string departmentName, string departmentCode)
        {
            DepartmentID = departmentId;
            DepartmentName = departmentName;
            DepartmentCode = departmentCode;
        }

        /// <summary>
        /// Unique identifier for the department.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Department code.
        /// </summary>
        public string? DepartmentCode { get; set; }

        /// <summary>
        /// Name of the department.
        /// </summary>
        public string? DepartmentName { get; set; }

        /// <summary>
        /// Address of the department.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// City.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Zip code.
        /// </summary>
        public string? Zip { get; set; }

        /// <summary>
        /// First name of the contact person.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name of the contact person.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string? Phone
        {
            get => _phone;
            set => _phone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Fax number.
        /// </summary>
        public string? Fax
        {
            get => _fax;
            set => _fax = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Email domain.
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// Whether the department is active.
        /// </summary>
        public bool? Active { get; set; }
        public string TimeZoneId { get; set; } = "Eastern Standard Time";

        /// <summary>
        /// Folder name for the department's employee Excel sheet(s) containing employee info data.
        /// </summary>
        public string? EmployeeDataFolderName { get; set; }

        /// <summary>
        /// Navigation property for the department policy.
        /// </summary>
        public DepartmentPolicyDto? DepartmentPolicy { get; set; }

        /// <summary>
        /// Navigation property for the department configuration.
        /// </summary>
        public DepartmentConfigurationDto? DepartmentConfiguration { get; set; }
        
        private string? _phone;
        private string? _fax;
    }
}

