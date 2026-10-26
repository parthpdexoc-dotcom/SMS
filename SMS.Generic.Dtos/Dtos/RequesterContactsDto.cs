using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for RequesterContacts operations.
    /// </summary>
    public class RequesterContactsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester contact.
        /// </summary>
        public long PkContactID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key department ID.
        /// </summary>
        public Guid? FkDepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key requester ID (references Requesters table).
        /// </summary>
        public Guid? FkRequesterID { get; set; }

        /// <summary>
        /// Gets or sets the foreign key contact type ID.
        /// </summary>
        public int? FkContactTypeID { get; set; }

        /// <summary>
        /// Gets or sets the contact name.
        /// </summary>
        [MaxLength(255, ErrorMessage = "ContactName cannot exceed 255 characters")]
        public string? ContactName { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [MaxLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
        public string? Phone
        {
            get => _phone;
            set => _phone = SMS.Generic.Common.Helpers.PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the phone extension.
        /// </summary>
        [MaxLength(5, ErrorMessage = "PhoneExt cannot exceed 5 characters")]
        public string? PhoneExt { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public short? Status { get; set; }

        /// <summary>
        /// Gets or sets the user ID who created this record.
        /// </summary>
        public long? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user ID who last modified this record.
        /// </summary>
        public long? ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the modification date.
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets whether this record is deleted.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets the user ID who deleted this record.
        /// </summary>
        public long? DeletedBy { get; set; }

        /// <summary>
        /// Gets or sets the deletion date.
        /// </summary>
        public DateTime? DeletedDate { get; set; }

        private string? _phone;
    }
}

