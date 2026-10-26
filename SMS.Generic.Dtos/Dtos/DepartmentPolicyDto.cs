using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentPolicy operations.
    /// </summary>
    public class DepartmentPolicyDto : DepartmentConfigurationDto
    {
        /// <summary>
        /// Unique identifier for the department policy.
        /// </summary>
        public Guid? DepartmentPolicyID { get; set; }
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

        // ===== Minimum Hours Paid =====
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
        /// When true, an employee in this department must have an admin type to sign in on the web. Defaults to false.
        /// </summary>
        public bool RestrictWebLoginForNonAdminEmployee { get; set; } = false;
        public int NumberOfEmployeesBeforeSupervisorRequired { get; set; }

        // ===== Tenure Policy =====
        public TenureMetricType? TenureType { get; set; }

        public string? TenureRulesJson { get; set; }

        /// <summary>
        /// Gets or sets the time to close post (e.g. minutes or hours).
        /// </summary>
        public int TimeToClosePost { get; set; }
    }
}
