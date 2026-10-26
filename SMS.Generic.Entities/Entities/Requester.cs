using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a requester entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(Name), OnlyAttributedProperties = true)]
    [Table("Requester", Schema = "dbo")]
    public class Requester : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the requester.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // because NEWID() is in SQL, not EF
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Unique code (nvarchar(20)) from dbo.Seq_RequesterCode; default is six-digit zero-padded (e.g. 000001).
        /// </summary>
        [MaxLength(20)]
        public string RequesterCode { get; set; } = null!;

        /// <summary>
        /// Gets or sets the department ID that this requester belongs to.
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the department defined number.
        /// </summary>
        [MaxLength(25)]
        [AuditedProperty("Department defined number")]
        public string? DepartmentDefinedNumber { get; set; }

        /// <summary>
        /// Gets or sets the business type ID.
        /// </summary>
        [Required]
        [AuditedProperty("Business type", LookupEntityType = typeof(BusinessType), LookupDisplayPropertyName = nameof(BusinessType.BusinessTypeLong))]
        public Guid BusinessTypeID { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [AuditedProperty("Status")]
        public bool? Status { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Address")]
        public string? Address { get; set; }

        /// <summary>
        /// Gets or sets the address line 2.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Address 2")]
        public string? Address2 { get; set; }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("City")]
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("State")]
        public string? State { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        [MaxLength(10)]
        [AuditedProperty("Zip")]
        public string? Zip { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the primary contact.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Primary contact")]
        public string? PrimaryContact { get; set; }

        /// <summary>
        /// Gets or sets the fax number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Fax")]
        public string? Fax { get; set; }

        /// <summary>
        /// Gets or sets the secondary contact.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Secondary contact")]
        public string? SecondaryContact { get; set; }

        /// <summary>
        /// Gets or sets the secondary phone number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Secondary phone")]
        public string? SecondaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the invoice billing name.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice billing name")]
        public string? InvoiceBillingName { get; set; }

        /// <summary>
        /// Gets or sets the invoice contact.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice contact")]
        public string? InvoiceContact { get; set; }

        /// <summary>
        /// Gets or sets the invoice address.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice address")]
        public string? InvoiceAddress { get; set; }

        /// <summary>
        /// Gets or sets the invoice address line 2.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice address 2")]
        public string? InvoiceAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the invoice city.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice city")]
        public string? InvoiceCity { get; set; }

        /// <summary>
        /// Gets or sets the invoice state.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice state")]
        public string? InvoiceState { get; set; }

        /// <summary>
        /// Gets or sets the invoice zip code.
        /// </summary>
        [MaxLength(10)]
        [AuditedProperty("Invoice zip")]
        public string? InvoiceZip { get; set; }

        /// <summary>
        /// Gets or sets the invoice phone number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice phone")]
        public string? InvoicePhone { get; set; }

        /// <summary>
        /// Gets or sets the invoice phone extension.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice phone text")]
        public string? InvoicePhoneExt { get; set; }

        /// <summary>
        /// Gets or sets the invoice email address.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice email")]
        public string? InvoiceEmail { get; set; }

        /// <summary>
        /// Gets or sets the invoice fax number.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Invoice fax")]
        public string? InvoiceFax { get; set; }

        /// <summary>
        /// Gets or sets whether liquor license is required.
        /// </summary>
        [AuditedProperty("Liquor license required")]
        public bool? IsLiquorLicenseRequired { get; set; }

        /// <summary>
        /// Gets or sets the liquor license (display if isLiquorLicenseRequired is true).
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Liquor license")]
        public string? LiquorLicense { get; set; }

        /// <summary>
        /// Gets or sets the billing notes.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Billing notes")]
        public string? BillingNotes { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Comments")]
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets whether approved for civilians.
        /// </summary>
        [AuditedProperty("Approved for civilians")]
        public bool? ApprovedForCivilians { get; set; }

        /// <summary>
        /// Gets or sets whether exempt from extract.
        /// </summary>
        [AuditedProperty("Exempt from extract")]
        public bool? ExemptFromExtract { get; set; }

        /// <summary>
        /// Gets or sets whether this is a child requester.
        /// </summary>
        [AuditedProperty("Is child")]
        public bool? IsChild { get; set; }

        /// <summary>
        /// Gets or sets whether the requester is authorized.
        /// </summary>
        [AuditedProperty("Requester authorized")]
        public bool? IsRequesterAuthorized { get; set; }

        /// <summary>
        /// Gets or sets the notification recipients.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Notification recipients")]
        public string? NotificationRecipients { get; set; }

        /// <summary>
        /// Gets or sets the parent requester ID (mandatory if IsChild is true).
        /// </summary>
        [AuditedProperty("Parent requester", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public Guid? ParentID { get; set; }

        /// <summary>
        /// Gets or sets the department questionnaire ID (foreign key to DepartmentQuestionnaire).
        /// </summary>
        public Guid? DepartmentQuestionnaireId { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the business type.
        /// </summary>
        [ForeignKey(nameof(BusinessTypeID))]
        [AuditedProperty("Business type", LookupEntityType = typeof(BusinessType), LookupDisplayPropertyName = nameof(BusinessType.BusinessTypeLong))]
        public virtual BusinessType? BusinessType { get; set; }

        /// <summary>
        /// Navigation property for the department questionnaire.
        /// </summary>
        [ForeignKey(nameof(DepartmentQuestionnaireId))]
        [AuditedProperty("Department questionnaire", LookupEntityType = typeof(DepartmentQuestionnaire), LookupDisplayPropertyName = nameof(DepartmentQuestionnaire.Questionnaire.Title))]
        public virtual DepartmentQuestionnaire? DepartmentQuestionnaire { get; set; }

        /// <summary>
        /// Navigation property for the parent requester (self-referencing).
        /// ParentID is the foreign key that references RequesterID (primary key).
        /// </summary>
        [AuditedProperty("Parent requester", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public virtual Requester? Parent { get; set; }

        /// <summary>
        /// Navigation property for child requester.
        /// </summary>
        [AuditedProperty("Children", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public virtual ICollection<Requester> Children { get; set; } = new List<Requester>();

        /// <summary>
        /// Navigation property for the employee who created this business type.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this business type.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        public virtual ICollection<RequesterManager> RequesterManagers { get; set; } = new List<RequesterManager>();

        /// <summary>
        /// Gets or sets the Skill IDs that are always required for this requester.
        /// These should be applied to all Posts under this requester and not removable at the Post level.
        /// </summary>
        [AuditedProperty("Skills required")]
        public bool IsSkillsRequired { get; set; }

        [AuditedProperty("Skills", GetSkillNamesByIds = true)]
        public List<Guid>? SkillIDs { get; set; }
    }
}
