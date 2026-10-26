using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a request entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(Title), OnlyAttributedProperties = true)]
    [Table("Request", Schema = "dbo")]
    public class Request : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequestID { get; set; }

        /// <summary>
        /// Unique code (nvarchar(20)) from dbo.Seq_RequestCode; default is six-digit zero-padded (e.g. 000001).
        /// </summary>
        [MaxLength(20)]
        [AuditedProperty("Request code")]
        public string RequestCode { get; set; } = null!;

        /// <summary>
        /// Gets or sets the requester ID (references Requesters table).
        /// </summary>
        [AuditedProperty("Requester", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public Guid RequesterID { get; set; }

        /// <summary>
        /// Gets or sets the requester manager ID (references RequesterManager table).
        /// Null means no requester manager selected (e.g., department admin-created request).
        /// </summary>
        [AuditedProperty("Requester manager", LookupEntityType = typeof(RequesterManager), LookupDisplayPropertyName = nameof(RequesterManager.EmployeeID))]
        public Guid? RequesterManagerID { get; set; }

        /// <summary>
        /// Navigation property for the requester.
        /// </summary>
        [ForeignKey(nameof(RequesterID))]
        public virtual Requester Requester { get; set; }

        /// <summary>
        /// Navigation property for the requester manager.
        /// </summary>
        [ForeignKey(nameof(RequesterManagerID))]
        public virtual RequesterManager? RequesterManager { get; set; }

        /// <summary>
        /// Gets or sets the department ID (references Department table).
        /// </summary>
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department Department { get; set; }


        /// <summary>
        /// Gets or sets the department response area ID (references DepartmentResponseArea table).
        /// </summary>
        [AuditedProperty("Department response area", LookupEntityType = typeof(DepartmentResponseArea), LookupDisplayPropertyName = nameof(DepartmentResponseArea.DepartmentResponseAreaShort))]
        public Guid? DepartmentResponseAreaID { get; set; }

        /// <summary>
        /// Navigation property for the department response area.
        /// </summary>
        [ForeignKey(nameof(DepartmentResponseAreaID))]
        public virtual DepartmentResponseArea? DepartmentResponseArea { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        public virtual ICollection<RequestSchedule> RequestSchedules { get; set; } = new HashSet<RequestSchedule>();
        public ICollection<WaitingPostPosition> WaitingPostPosition { get; set; } = new List<WaitingPostPosition>();

        /// <summary>
        /// Gets or sets whether the request is tenure based.
        /// </summary>
        [AuditedProperty("Enable waiting list")]
        public bool EnableWaitingList { get; set; }

        /// <summary>
        /// Gets or sets whether the request has been updated.
        /// </summary>
        [AuditedProperty("Request has been updated")]
        public bool? RequestHasBeenUpdated { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("First name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Last name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the primary phone.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Primary phone")]
        public string? PrimaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the cell phone.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Cell phone")]
        public string? CellPhone { get; set; }

        /// <summary>
        /// Gets or sets the secondary phone.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Secondary phone")]
        public string? SecondaryPhone { get; set; }

        /// <summary>
        /// Gets or sets the alternate cell phone.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Alternate cell phone")]
        public string? AlternateCellPhone { get; set; }

        /// <summary>
        /// Gets or sets the primary email.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Primary email")]
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Gets or sets the secondary email.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Secondary email")]
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// Gets or sets whether job contact is required.
        /// </summary>
        [AuditedProperty("Is job contact")]
        public bool? IsPostContact { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        [Column(TypeName = "date")]
        [AuditedProperty("Start date")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        [Column(TypeName = "date")]
        [AuditedProperty("End date")]
        public DateOnly EndDate { get; set; }

        /// <summary>
        /// Gets or sets how the request is released.
        /// Stored as int because it maps to the ReleaseAs enum.
        /// </summary>
        [AuditedProperty("Release as")]
        public ReleaseAs ReleaseAs { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Title")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Location")]
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the request site street address (required).
        /// </summary>
        [Required]
        [MaxLength(250)]
        [AuditedProperty("Address")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request site city (required).
        /// </summary>
        [Required]
        [MaxLength(50)]
        [AuditedProperty("City")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request site state (required).
        /// </summary>
        [Required]
        [MaxLength(50)]
        [AuditedProperty("State")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request site ZIP or postal code (required).
        /// </summary>
        [Required]
        [MaxLength(10)]
        [AuditedProperty("Zip")]
        public string Zip { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether job locations are stationary.
        /// </summary>
        [AuditedProperty("Job locations stationary")]
        public bool? PostLocationsAreStationary { get; set; }

        /// <summary>
        /// Gets or sets whether a permit is required.
        /// </summary>
        [AuditedProperty("Requires permit")]
        public bool? RequiresPermit { get; set; }

        /// <summary>
        /// Gets or sets whether the request is a special event.
        /// </summary>
        [AuditedProperty("Special event")]
        public bool? IsSpecialEvent { get; set; }

        /// <summary>
        /// Gets or sets whether the special event is approved.
        /// </summary>
        [AuditedProperty("Special event approved")]
        public bool? SpecialEventApproved { get; set; }

        /// <summary>
        /// Gets or sets whether a liquor license is required.
        /// </summary>
        [AuditedProperty("Requires liquor license")]
        public bool? RequiresLiquorLicense { get; set; }

        /// <summary>
        /// Gets or sets whether an alcohol permit is required.
        /// </summary>
        [AuditedProperty("Alcohol permit required")]
        public bool AlcoholPermitRequired { get; set; }

        /// <summary>
        /// Gets or sets the special rate for this request.
        /// </summary>
        [Column(TypeName = "money")]
        [AuditedProperty("Special rate")]
        public decimal SpecialRate { get; set; }

        /// <summary>
        /// Gets or sets whether a vehicle is required.
        /// </summary>
        [AuditedProperty("Requires vehicle")]
        public bool? RequiresVehicle { get; set; }

        /// <summary>
        /// Gets or sets whether the vehicle requirement is approved.
        /// </summary>
        [AuditedProperty("Vehicle approved")]
        public bool? VehicleApproved { get; set; }

        /// <summary>
        /// Gets or sets whether equipment is required.
        /// </summary>
        [AuditedProperty("Requires equipment")]
        public bool? RequiresEquipment { get; set; }

        /// <summary>
        /// Gets or sets whether a skill is required.
        /// </summary>
        [AuditedProperty("Skill required")]
        public bool? IsSkillRequired { get; set; }

        /// <summary>
        /// Gets or sets whether a skill is required.
        /// </summary>
        [AuditedProperty("Skills", GetSkillNamesByIds = true)]
        public List<Guid>? SkillIDs { get; set; }

        [AuditedProperty("Liquor license number")]
        public string? LiquorLicenseNumber { get; set; }
        [AuditedProperty("Permit number")]
        public string? PermitNumber { get; set; }

        /// <summary>
        /// Navigation property for Recurrence rules / Request Type details.
        /// </summary>
        public virtual RequestType? RequestType { get; set; }

        // Removed RequestType enum property as it was dropped in migration.

        /// <summary>
        /// Gets or sets the department defined field value 1.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 1")]
        public string? DepartmentDefinedFieldValue1 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 2.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 2")]
        public string? DepartmentDefinedFieldValue2 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 3.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 3")]
        public string? DepartmentDefinedFieldValue3 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 4.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 4")]
        public string? DepartmentDefinedFieldValue4 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 5.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 5")]
        public string? DepartmentDefinedFieldValue5 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 6.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 6")]
        public string? DepartmentDefinedFieldValue6 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 7.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 7")]
        public string? DepartmentDefinedFieldValue7 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 8.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 8")]
        public string? DepartmentDefinedFieldValue8 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 9.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 9")]
        public string? DepartmentDefinedFieldValue9 { get; set; }

        /// <summary>
        /// Gets or sets the department defined field value 10.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined field value 10")]
        public string? DepartmentDefinedFieldValue10 { get; set; }

        /// <summary>
        /// Gets or sets the fiscal year.
        /// </summary>
        [AuditedProperty("Fiscal year")]
        public int? FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets the project number.
        /// </summary>
        [AuditedProperty("Project number")]
        public string? ProjectNumber { get; set; }

        /// <summary>
        /// Gets or sets whether this request is exempt from hour-based policy rules.
        /// </summary>
        [AuditedProperty("Is hours policy exempt")]
        public bool IsHoursPolicyExempt { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this request is a premium paid event.
        /// </summary>
        [AuditedProperty("Premium paid event")]
        public bool IsPremiumPaidEvent { get; set; } = false;

        /// <summary>
        /// Gets or sets whether department fees apply to this request.
        /// </summary>
        [AuditedProperty("Enable fees")]
        public bool EnableFees { get; set; } = true;

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
    }
}