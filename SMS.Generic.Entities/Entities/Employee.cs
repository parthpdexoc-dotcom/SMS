using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an employee entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(FirstName), OnlyAttributedProperties = true)]
    [Table("Employee", Schema = "dbo")]
    public class Employee : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Unique code (nvarchar(20)) from dbo.Seq_EmployeeCode; default is six-digit zero-padded (e.g. 000001).
        /// </summary>
        [MaxLength(20)]
        [AuditedProperty("Employee code")]
        public string EmployeeCode { get; set; } = null!;

        [MaxLength(50)]
        [AuditedProperty("Employee number")]
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Foreign Key to Department
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Foreign Key to Employee Type
        /// </summary>
        [Required]
        [AuditedProperty("Employee type", LookupEntityType = typeof(EmployeeType), LookupDisplayPropertyName = nameof(EmployeeType.EmployeeTypeLong))]
        public Guid EmployeeTypeID { get; set; }

        /// <summary>
        /// BadgeNumber
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Badge number")]
        public string? BadgeNumber { get; set; }

        /// <summary>
        /// StatusID (optional - foreign key to EmployeeStatus).
        /// </summary>
        [AuditedProperty("Status", LookupEntityType = typeof(EmployeeStatus), LookupDisplayPropertyName = nameof(EmployeeStatus.StatusShort))]
        public Guid? StatusID { get; set; }

        /// <summary>
        /// Indicates whether the employee is approved.
        /// </summary>
        [AuditedProperty("Is approved")]
        public bool IsApproved { get; set; } = true;

        /// <summary>
        /// Indicates whether the employee has a subsidized vehicle.
        /// </summary>
        [AuditedProperty("Is subsidized vehicle")]
        public bool IsSubsidizedVehicle { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        [MaxLength(100)]
        [AuditedProperty("First name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [MaxLength(100)]
        [AuditedProperty("Last name")]
        public string? LastName { get; set; }

        [NotMapped]
        [AuditedProperty("Full name")]
        public string FullName => $"{LastName}, {FirstName}".Trim();

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [MaxLength(25)]
        [AuditedProperty("Username")]
        public string? Username { get; set; }

        /// <summary>
        /// Password (BCrypt hashed)
        /// </summary>
        [Required]
        [MaxLength(60)]
        public string? Password { get; set; }

        public Guid? ActiveSessionId { get; set; }

        /// <summary>
        /// Primary phone number
        /// </summary>
        [MaxLength(25)]
        [AuditedProperty("Primary phone")]
        public string? PrimaryPhone { get; set; }

        /// <summary>
        /// Secondary phone number
        /// </summary>
        [MaxLength(25)]
        [AuditedProperty("Secondary phone")]
        public string? SecondaryPhone { get; set; }

        /// <summary>
        /// Primary email address
        /// </summary>
        [MaxLength(150)]
        [AuditedProperty("Primary email")]
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Secondary email address
        /// </summary>
        [MaxLength(150)]
        [AuditedProperty("Secondary email")]
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// Phone number for off-duty call back (separate from primary/secondary work contact).
        /// </summary>
        [MaxLength(25)]
        [AuditedProperty("Off-duty call back phone number")]
        public string? OffDutyCallBackPhoneNumber { get; set; }

        /// <summary>
        /// Phone number used for SMS notifications.
        /// </summary>
        [MaxLength(25)]
        [AuditedProperty("SMS telephone number")]
        public string? SmsTelephoneNumber { get; set; }

        /// <summary>
        /// When true, the employee has accepted the SMS consent disclosure.
        /// </summary>
        [AuditedProperty("SMS consent")]
        public bool HasSmsConsent { get; set; } = false;

        /// <summary>
        /// Email for off-duty call back (separate from primary/secondary email).
        /// </summary>
        [MaxLength(150)]
        [AuditedProperty("Off-duty call back email")]
        public string? OffDutyCallBackEmail { get; set; }

        /// <summary>
        /// When true, employee receives email when new jobs are posted/released. False = no notification.
        /// </summary>
        [AuditedProperty("Notify when jobs posted")]
        public bool NotifyWhenJobsPosted { get; set; } = false;

        /// <summary>
        /// When true, employee receives email reminder 24 hours before a job starts. False = no notification.
        /// </summary>
        [AuditedProperty("Notify job starting in 24 hours")]
        public bool NotifyJobStartingIn24Hours { get; set; } = false;

        /// <summary>
        /// When true, employee receives email reminder 1 hour before a job starts. False = no notification.
        /// </summary>
        [AuditedProperty("Notify job starting in 1 hour")]
        public bool NotifyJobStartingIn1Hour { get; set; } = false;

        /// <summary>
        /// OTP for forgot password flow (sent to email, verified then cleared).
        /// </summary>
        [MaxLength(10)]
        public string? Otp { get; set; }

        /// <summary>
        /// Indicates whether the user has completed a password reset flow.
        /// </summary>
        [Column("isPasswordReset")]
        [AuditedProperty("Is password reset")]
        public bool IsPasswordReset { get; set; } = false;

        /// <summary>
        /// Hourly salary
        /// </summary>
        [Column(TypeName = "decimal(18,5)")]
        [AuditedProperty("Hourly salary")]
        public decimal? HourlySalary { get; set; }

        /// <summary>
        /// HR Cost Center Code
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("HR cost center code")]
        public string? HrCostCenterCode { get; set; }

        /// <summary>
        /// Foreign Key to Department Divisions
        /// </summary>
        [AuditedProperty("Division", LookupEntityType = typeof(DepartmentDivisions), LookupDisplayPropertyName = nameof(DepartmentDivisions.DivisionLong))]
        public Guid? DivisionID { get; set; }

        /// <summary>
        /// Employee Department Defined Field 1
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined 1")]
        public string? EmployeeDepartmentDefinedField1 { get; set; }

        /// <summary>
        /// Employee Department Defined Field 2
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined 2")]
        public string? EmployeeDepartmentDefinedField2 { get; set; }

        /// <summary>
        /// Employee Department Defined Field 3
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined 3")]
        public string? EmployeeDepartmentDefinedField3 { get; set; }

        /// <summary>
        /// Employee Department Defined Field 4
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Department defined 4")]
        public string? EmployeeDepartmentDefinedField4 { get; set; }
        
        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee type.
        /// </summary>
        [ForeignKey(nameof(EmployeeTypeID))]
        public virtual EmployeeType? EmployeeType { get; set; }

        /// <summary>
        /// Navigation property for the employee status.
        /// </summary>
        [ForeignKey(nameof(StatusID))]
        public virtual EmployeeStatus? Status { get; set; }

        /// <summary>
        /// Navigation property for the department division.
        /// </summary>
        [ForeignKey(nameof(DivisionID))]
        public virtual DepartmentDivisions? Division { get; set; }
        /// <summary>
        /// Navigation property for the employee who created this skill.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this skill.
        /// </summary>
        public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for employee admin records associated with this employee.
        /// </summary>
        public virtual ICollection<EmployeeAdmin> EmployeeAdmins { get; set; } = new List<EmployeeAdmin>();

        /// <summary>
        /// Navigation property for employee admin records created by this employee.
        /// </summary>
        public virtual ICollection<EmployeeAdmin> CreatedEmployeeAdmins { get; set; } = new List<EmployeeAdmin>();

        /// <summary>
        /// Navigation property for employee admin records edited by this employee.
        /// </summary>
        public virtual ICollection<EmployeeAdmin> EditedEmployeeAdmins { get; set; } = new List<EmployeeAdmin>();

        /// <summary>
        /// Navigation property for department admin records where this employee is the assigned admin.
        /// </summary>
        public virtual ICollection<DepartmentAdmin> DepartmentAdmins { get; set; } = new List<DepartmentAdmin>();
    }
}



