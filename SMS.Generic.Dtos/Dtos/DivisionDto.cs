using System.ComponentModel.DataAnnotations;
using SMS.Generic.Common.Helpers;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Division operations.
    /// </summary>
    public class DivisionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the division.
        /// </summary>
        public int DivisionID { get; set; }

        /// <summary>
        /// Gets or sets the department ID that this division belongs to.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the division name.
        /// </summary>
        public string? DivisionName { get; set; }

        /// <summary>
        /// Gets or sets the division description.
        /// </summary>
        public string? DivisionDescription { get; set; }

        /// <summary>
        /// Gets or sets the contact person name.
        /// </summary>
        public string? Contact { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string? Phone
        {
            get => _phone;
            set => _phone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the private line number.
        /// </summary>
        public string? PvtLine { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the billing name.
        /// </summary>
        public string? NmBilling { get; set; }

        /// <summary>
        /// Gets or sets whether the division is a member of agency.
        /// </summary>
        public bool? MemberOfAgency { get; set; }
        
        private string? _phone;
    }
}



