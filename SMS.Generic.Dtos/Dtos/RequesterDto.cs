using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Requesters operations.
    /// </summary>
    public class RequesterDto : BaseDto, IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester.
        /// If null or empty, a new Guid will be generated when creating a new record.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("requesterID")]
        [System.Text.Json.Serialization.JsonPropertyName("requesterID")]
        public Guid? RequesterID { get; set; }

        /// <summary>
        /// Human-readable requester code from sequence (e.g. 000001).
        /// </summary>
        public string? RequesterCode { get; set; }

        /// <summary>
        /// Gets or sets the department ID that this requester belongs to.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the department defined number.
        /// </summary>
        public string? DepartmentDefinedNumber { get; set; }

        /// <summary>
        /// Gets or sets the business type ID.
        /// </summary>
        public Guid? BusinessTypeID { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        public string? Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        public string? Zip { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string? Phone
        {
            get => _phone;
            set => _phone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the primary contact.
        /// </summary>
        public string? PrimaryContact { get; set; }

        /// <summary>
        /// Gets or sets the fax number.
        /// </summary>
        public string? Fax
        {
            get => _fax;
            set => _fax = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the secondary contact.
        /// </summary>
        public string? SecondaryContact { get; set; }

        /// <summary>
        /// Gets or sets the secondary phone number.
        /// </summary>
        public string? SecondaryPhone
        {
            get => _secondaryPhone;
            set => _secondaryPhone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the invoice billing name.
        /// </summary>
        public string? InvoiceBillingName { get; set; }

        /// <summary>
        /// Gets or sets the invoice contact.
        /// </summary>
        public string? InvoiceContact { get; set; }

        /// <summary>
        /// Gets or sets the invoice address.
        /// </summary>
        public string? InvoiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the invoice address line 2.
        /// </summary>
        public string? InvoiceAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the invoice city.
        /// </summary>
        public string? InvoiceCity { get; set; }

        /// <summary>
        /// Gets or sets the invoice state.
        /// </summary>
        public string? InvoiceState { get; set; }

        /// <summary>
        /// Gets or sets the invoice zip code.
        /// </summary>
        public string? InvoiceZip { get; set; }

        /// <summary>
        /// Gets or sets the invoice phone number.
        /// </summary>
        public string? InvoicePhone
        {
            get => _invoicePhone;
            set => _invoicePhone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the invoice phone extension.
        /// </summary>
        public string? InvoicePhoneExt
        {
            get => _invoicePhoneExt;
            set => _invoicePhoneExt = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets the invoice email address.
        /// </summary>
        public string? InvoiceEmail { get; set; }

        /// <summary>
        /// Gets or sets the invoice fax number.
        /// </summary>
        public string? InvoiceFax
        {
            get => _invoiceFax;
            set => _invoiceFax = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Gets or sets whether liquor license is required.
        /// </summary>
        public bool? IsLiquorLicenseRequired { get; set; }

        /// <summary>
        /// Gets or sets the liquor license (display if RequiresLiquorLicense is true).
        /// </summary>
        public string? LiquorLicense { get; set; }

        /// <summary>
        /// Gets or sets the billing notes.
        /// </summary>
        public string? BillingNotes { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets whether approved for civilians.
        /// </summary>
        public bool? ApprovedForCivilians { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from extract.
        /// </summary>
        public bool? ExemptFromExtract { get; set; }

        /// <summary>
        /// Gets or sets whether this is a child requester.
        /// </summary>
        public bool? IsChild { get; set; }

        /// <summary>
        /// Gets or sets whether the requester is authorized.
        /// </summary>
        public bool? IsRequesterAuthorized { get; set; }

        /// <summary>
        /// Gets or sets the notification recipients.
        /// </summary>
        public string? NotificationRecipients { get; set; }

        /// <summary>
        /// Gets or sets the parent requester ID (mandatory if IsChild is true).
        /// </summary>
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the department questionnaire ID (foreign key to DepartmentQuestionnaire).
        /// </summary>
        public Guid? DepartmentQuestionnaireId { get; set; }

        public bool IsSkillsRequired { get; set; }


        /// <summary>
        /// Gets or sets the Skill IDs that are always required for this requester.
        /// These should be applied to all Posts under this requester and not removable at the Post level.
        /// </summary>
        public List<Guid>? SkillIDs { get; set; }
        public List<RequesterManagerDto>? RequesterManagers { get; set; }

        private string? _phone;
        private string? _fax;
        private string? _secondaryPhone;
        private string? _invoicePhone;
        private string? _invoicePhoneExt;
        private string? _invoiceFax;
        public bool IsEmployeeManager(Guid employeeId)
            => RequesterManagers?.Any(m => m.IsManagerEmployee(employeeId)) == true;

        public RequesterManagerDto? GetManager(Guid managerId)
            => RequesterManagers?.FirstOrDefault(m => m.IsManager(managerId));

        /// <summary>
        /// Returns true when employee belongs to creator manager scope.
        /// </summary>
        public bool IsEmployeeUnderCreatorScope(Guid creatorManagerId, Guid employeeId)
            => GetManager(creatorManagerId)?.ContainsEmployee(employeeId) == true;
    }

    public class RequesterResponseDto : RequesterDto
    {
        public string? BusinessType { get; set; }
    }
}

