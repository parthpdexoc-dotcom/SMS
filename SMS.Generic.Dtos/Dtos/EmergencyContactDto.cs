using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for EmergencyContact operations.
    /// </summary>
    public class EmergencyContactDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the emergency contact.
        /// </summary>
        public int EmergencyContactID { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public int? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the name of the emergency contact.
        /// </summary>
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the relationship to the employee.
        /// </summary>
        [MaxLength(30, ErrorMessage = "Relationship cannot exceed 30 characters")]
        public string? Relationship { get; set; }

        /// <summary>
        /// Gets or sets the primary phone number.
        /// </summary>
        [MaxLength(30, ErrorMessage = "Phone1 cannot exceed 30 characters")]
        public string? Phone1
        {
            get => _phone1;
            set => _phone1 = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the secondary phone number.
        /// </summary>
        [MaxLength(30, ErrorMessage = "Phone2 cannot exceed 30 characters")]
        public string? Phone2
        {
            get => _phone2;
            set => _phone2 = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        [MaxLength(150, ErrorMessage = "Comments cannot exceed 150 characters")]
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets whether the contact is approved.
        /// </summary>
        public bool? Approve { get; set; }

        /// <summary>
        /// Gets or sets the list order.
        /// </summary>
        public short? ListOrder { get; set; }

        /// <summary>
        /// Gets or sets who added the contact.
        /// </summary>
        [MaxLength(50, ErrorMessage = "AddedBy cannot exceed 50 characters")]
        public string? AddedBy { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime? TimeStamp { get; set; }
        
        private string? _phone1;
        private string? _phone2;
    }
}

