using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department policy entity in the system.
    /// </summary>
    [Table("DepartmentPolicies", Schema = "dbo")]
    public class DepartmentPolicy
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department policy.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentPolicyID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        public Guid DepartmentID { get; set; }
        // ===== Enable / Disable Flags =====
        public bool IsPostsTakenPerDay { get; set; }
        public bool IsHoursPerDayWorked { get; set; }
        public bool IsHoursPerWeekWorked { get; set; }
        public bool IsHoursPerMonthWorked { get; set; }
        public bool IsDeptOpenPostsTakenPerDay { get; set; }
        public bool IsTenurePostsAllowedPerWeek { get; set; }
        public bool IsTenurePostsAllowedPerDay { get; set; }
        public bool IsDeptOpenPostsTakenPerWeek { get; set; }

        // ===== Limits =====
        public int PostsTakenPerDay { get; set; }
        public int HoursPerDayWorked { get; set; }
        public int HoursPerWeekWorked { get; set; }
        public int HoursPerMonthWorked { get; set; }
        public int DeptOpenPostsTakenPerDay { get; set; }
        public int TenurePostsAllowedPerWeek { get; set; }
        public int TenurePostsAllowedPerDay { get; set; }
        public int DeptOpenPostsTakenPerWeek { get; set; }

        // ===== Preview =====
        public bool? IsPreviewable { get; set; }
        public short? PreviewableHours { get; set; }

        // ===== Min Hours Paid =====
        public bool IsMinHoursPaid { get; set; }
        public int MinHoursPaid { get; set; }

        // ===== Same Day Rules =====
        public bool IsPositionTakenOnSameday { get; set; }
        public short PositionTakenOnSameday { get; set; }

        public bool? IsPostExemptTakenOnSameday { get; set; }
        public bool? IsManagedPostTakenOnSameday { get; set; }
        public short? ManagedPostTakenOnSameday { get; set; }

        // ===== Supervisor / Overlap Rules =====
        public int? PostToSupervise { get; set; }
        public bool OverlapJobConflictConfirmation { get; set; }
        public bool SupervisorsCanViewLowerEmpTypeJobs { get; set; }

        /// <summary>
        /// When true, an employee in this department must have an admin type to sign in on the web. Mobile login is unchanged. Defaults to false.
        /// </summary>
        public bool RestrictWebLoginForNonAdminEmployee { get; set; } = false;
        public int NumberOfEmployeesBeforeSupervisorRequired { get; set; }

        // ===== Tenure Policy =====
        public TenureMetricType? TenureType { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? TenureRulesJson { get; set; }

        /// <summary>
        /// Gets or sets the time to close post (e.g. minutes or hours).
        /// </summary>
        public int? TimeToClosePost { get; set; }

        /// <summary>
        /// Record added timestamp.
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// User who added record.
        /// </summary>
        public Guid? CreatedUserID { get; set; }

        /// <summary>
        /// Name of user who added record.
        /// </summary>
        [MaxLength(255)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Edit timestamp.
        /// </summary>
        public DateTime? EditedDate { get; set; }

        /// <summary>
        /// User who edited record.
        /// </summary>
        public Guid? EditedUserID { get; set; }

        /// <summary>
        /// Name of user editing record.
        /// </summary>
        [MaxLength(255)]
        public string? EditedBy { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this policy.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this policy.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the department configuration.
        /// Relationship is configured via shared DepartmentID in DbContext.
        /// </summary>
        public virtual DepartmentConfiguration? DepartmentConfiguration { get; set; }
    }
}
