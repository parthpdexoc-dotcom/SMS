using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentConfiguration operations.
    /// </summary>
    public class DepartmentConfigurationDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department configuration.
        /// </summary>
        public Guid? DeptConfigID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the default pay rate.
        /// </summary>
        public decimal DefaultPayRate { get; set; }

        /// <summary>
        /// Gets or sets the admin fee rate.
        /// </summary>
        public decimal AdminFeeRate { get; set; }

        /// <summary>
        /// Gets or sets the default vehicle rate.
        /// </summary>
        public decimal DefaultVehicleRate { get; set; }

        /// <summary>
        /// Gets or sets the on duty start day of week.
        /// </summary>
        public short OnDutyStartDayOfWeek { get; set; }

        /// <summary>
        /// Gets or sets whether tenure is enabled.
        /// </summary>
        public bool Tenure { get; set; }

        /// <summary>
        /// Gets or sets the tenure window.
        /// </summary>
        public short TenureWindow { get; set; }

        /// <summary>
        /// Gets or sets whether reimbursement is enabled.
        /// </summary>
        public bool Reimburse { get; set; }

        /// <summary>
        /// Gets or sets whether invoicing is enabled.
        /// </summary>
        public bool Invoicing { get; set; }

        /// <summary>
        /// Gets or sets whether employee transfers are allowed.
        /// </summary>
        public bool AllowEmployeeTransfers { get; set; }

        /// <summary>
        /// Gets or sets whether 24 hour is based on time of day.
        /// </summary>
        public bool TwentyFourHourBasedOnTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets whether employees can submit forms.
        /// </summary>
        public bool CanEmployeeSubmitForm { get; set; }

        /// <summary>
        /// Gets or sets the allowable non-closed days.
        /// </summary>
        public short AllowableNonClosedDays { get; set; }

        /// <summary>
        /// Gets or sets whether activity types are enabled.
        /// </summary>
        public bool EnableActivityTypes { get; set; }

        /// <summary>
        /// Gets or sets the standard offset.
        /// </summary>
        public int StandardOffset { get; set; }

        /// <summary>
        /// Gets or sets the password changed period.
        /// </summary>
        public int PasswordChangedPeriod { get; set; }

        /// <summary>
        /// Gets or sets whether post definition is enabled.
        /// </summary>
        public bool IsDefinePost { get; set; }

        /// <summary>
        /// Gets or sets whether supervisor override is enabled.
        /// </summary>
        public bool IsSupervisorOverride { get; set; }

        /// <summary>
        /// Gets or sets the supervisor override hour.
        /// </summary>
        public int SupervisorOverrideHour { get; set; }

        /// <summary>
        /// Gets or sets whether to send notifications to all on transfer.
        /// </summary>
        public bool SendNotificationsToAllOnTransfer { get; set; }

        /// <summary>
        /// Gets or sets the reserves allowed hours per week.
        /// </summary>
        public byte ReservesAllowedHoursPerWeek { get; set; }

        /// <summary>
        /// Gets or sets whether skills are enabled.
        /// </summary>
        public bool Skills { get; set; }

        /// <summary>
        /// Gets or sets whether equipment is enabled.
        /// </summary>
        public bool Equipment { get; set; }

        /// <summary>
        /// Gets or sets whether cancellation settings are enabled.
        /// </summary>
        public bool CancellationSettings { get; set; }

        /// <summary>
        /// Gets or sets whether override settings are enabled.
        /// </summary>
        public bool OverrideSettings { get; set; }

        /// <summary>
        /// Gets or sets whether notification settings are enabled.
        /// </summary>
        public bool NotificationSettings { get; set; }

        /// <summary>
        /// Gets or sets whether close request settings are enabled.
        /// </summary>
        public bool CloseRequestSettings { get; set; }

        /// <summary>
        /// Gets or sets whether requester employee restricted list is enabled.
        /// </summary>
        public bool EnableRequesterEmployeeRestrictedList { get; set; }

        /// <summary>
        /// Gets or sets whether pre-employment paperwork is required.
        /// </summary>
        public bool EnablePreEmploymentPaperworkRequired { get; set; }

        /// <summary>
        /// Gets or sets whether requester specific configurations are enabled.
        /// </summary>
        public bool EnableRequesterSpecificConfigurations { get; set; }

        /// <summary>
        /// Gets or sets whether payroll configurations are enabled.
        /// </summary>
        public bool EnablePayrollConfigurations { get; set; }

        /// <summary>
        /// Gets or sets whether the payroll cron job is enabled.
        /// </summary>
        public bool EnablePayrollCronJob { get; set; }

        /// <summary>
        /// When true, PTAEO code fields are shown in the UI; when false, they are hidden.
        /// </summary>
        public bool ShowPtaeoCode { get; set; } = true;

        /// <summary>
        /// When true, pay code fields are shown in the UI; when false, they are hidden.
        /// </summary>
        public bool ShowPayCode { get; set; } = true;

        /// <summary>
        /// When true, the Generate Schedule action is available for this department.
        /// </summary>
        public bool EnableGenerateRequestSchedule { get; set; } = true;

        /// <summary>
        /// Gets or sets whether policy management is enabled.
        /// </summary>
        public bool EnablePolicyManagement { get; set; }

        /// <summary>
        /// Gets or sets whether schedule priority management is enabled.
        /// </summary>
        public bool ManageSchedulePriority { get; set; }

        /// <summary>
        /// Gets or sets whether schedule priority list management is enabled.
        /// </summary>
        public bool ManageSchedulePriorityList { get; set; }

        /// <summary>
        /// Gets or sets whether schedule priority released list management is enabled.
        /// </summary>
        public bool ManageSchedulePriorityReleasedList { get; set; }

        /// <summary>
        /// Gets or sets whether schedule priority snapshot is enabled.
        /// </summary>
        public bool SchedulePrioritySnapshot { get; set; }

        /// <summary>
        /// Gets or sets whether department scheduling priority is enabled.
        /// </summary>
        public bool DepartmentSchedulingPriority { get; set; }

        /// <summary>
        /// Gets or sets whether requester scheduling priority is enabled.
        /// </summary>
        public bool RequesterSchedulingPriority { get; set; }

        /// <summary>
        /// Gets or sets whether coordinated request scheduling priority is enabled.
        /// </summary>
        public bool CoordinatedRequestSchedulingPriority { get; set; }

        /// <summary>
        /// Gets or sets whether employee cancellation is allowed.
        /// </summary>
        public bool AllowEmployeeCancellation { get; set; }

        /// <summary>
        /// Gets or sets whether self-cancellation is allowed within a time period.
        /// </summary>
        public bool SelfCancellationAllowedWithin { get; set; }

        /// <summary>
        /// Gets or sets whether pre-employment paperwork upload is enabled.
        /// </summary>
        public bool UploadPreEmploymentPaperwork { get; set; }

        /// <summary>
        /// Gets or sets whether communications management is enabled.
        /// </summary>
        public bool ManageCommunications { get; set; }

        /// <summary>
        /// Gets or sets whether requester page is enabled.
        /// </summary>
        public bool RequesterPage { get; set; }

        /// <summary>
        /// Gets or sets whether skill management is enabled.
        /// </summary>
        public bool SkillManagement { get; set; }

        /// <summary>
        /// Gets or sets whether employee vehicle reimbursement is enabled.
        /// </summary>
        public bool EmployeeVehicleReimbursement { get; set; }

        /// <summary>
        /// Gets or sets whether employee mileage reimbursement is enabled.
        /// </summary>
        public bool EmployeeMileageReimbursement { get; set; }

        /// <summary>
        /// Gets or sets whether tenure-based post 24-hour limit is enabled.
        /// </summary>
        public bool EnableTenureBasedPost24HourLimit { get; set; }

        /// <summary>
        /// Gets or sets whether tenure-based post 24-hour allowed is enabled.
        /// </summary>
        public bool EnableTenureBasedPost24Allowed { get; set; }

        /// <summary>
        /// Gets or sets whether tenure-based post position is enabled.
        /// </summary>
        public bool EnableTenureBasedPostPosition { get; set; }

        /// <summary>
        /// Gets or sets whether tenure self-cancellation is enabled.
        /// </summary>
        public bool TenureSelfCancel { get; set; }

        /// <summary>
        /// Gets or sets whether the tenure wait list is considered on cancellation.
        /// </summary>
        public bool ConsiderTenureWaitListOnCancellation { get; set; }

        /// <summary>
        /// Gets or sets whether seniority based on private jobs is enabled.
        /// </summary>
        public bool EnableSeniorityBasedonPrivateJobs { get; set; }

        /// <summary>
        /// Gets or sets whether the open wait list is considered on cancellation.
        /// </summary>
        public bool ConsiderOpenWaitListOnCancellation { get; set; }

        /// <summary>
        /// Gets or sets whether open agency wait lists are enabled.
        /// </summary>
        public bool EnableOpenAgencyWaitLists { get; set; }

        /// <summary>
        /// Gets or sets whether private job wait lists are enabled.
        /// </summary>
        public bool EnablePrivateJobWaitLists { get; set; }

        /// <summary>
        /// Gets or sets whether employee submitting requests for approval is enabled.
        /// </summary>
        public bool EnableEmployeeSubmittingRequestsForApproval { get; set; }

        /// <summary>
        /// Gets or sets whether employee splitting posts is enabled.
        /// </summary>
        public bool EnableEmployeeSplittingPosts { get; set; }

        /// <summary>
        /// Gets or sets whether reporting hours worked as on or off duty is enabled.
        /// </summary>
        public bool EnableReportingHoursWorkedAsOnOrOffDuty { get; set; }

        /// <summary>
        /// Gets or sets whether supervisor is required.
        /// </summary>
        public bool IsSupervisorRequired { get; set; }

        /// <summary>
        /// Gets or sets the number of positions per supervisor.
        /// </summary>
        public int NumberOfPositionsPerSupervisor { get; set; }

        /// <summary>
        /// Gets or sets whether automatic suspension of employee for policy violations is enabled.
        /// </summary>
        public bool EnableAutomaticSuspensionOfEmployeeForPolicyViolations { get; set; }

        /// <summary>
        /// Gets or sets whether department exemptions are enabled.
        /// </summary>
        public bool EnableDepartmentExemptions { get; set; }

        /// <summary>
        /// Gets or sets whether requester specific skills are enabled.
        /// </summary>
        public bool RequesterSpecificSkills { get; set; }

        /// <summary>
        /// Gets or sets whether requester approval process is required.
        /// </summary>
        public bool RequesterApprovalProcessRequired { get; set; }

        /// <summary>
        /// Gets or sets whether shared scheduling priority pools are enabled.
        /// </summary>
        public bool SharedSchedulingPriorityPools { get; set; }

        /// <summary>
        /// Gets or sets whether agency defined fields are used.
        /// </summary>
        public bool UseAgencyDefinedFields { get; set; }

        /// <summary>
        /// Gets or sets whether liquor license tracking is enabled.
        /// </summary>
        public bool EnableLiquorLicenseTracking { get; set; }

        /// <summary>
        /// Gets or sets whether attaching files to requests is enabled.
        /// </summary>
        public bool EnableAttachFilesToRequests { get; set; }

        /// <summary>
        /// Gets or sets whether overlap job conflict confirmation is enabled.
        /// </summary>
        public bool EnableOverlapJobConflictConfirmation { get; set; }

        /// <summary>
        /// Gets or sets whether supervisors viewing lower employee type jobs is enabled.
        /// </summary>
        public bool EnableSupervisorsViewingLowerEmpTypeJobs { get; set; }

        /// <summary>
        /// Gets or sets whether employee edit start/end times after post begins is enabled.
        /// </summary>
        public bool EnableEmployeeEditStartEndTimesAfterPostBegins { get; set; }

        /// <summary>
        /// Gets or sets whether department edit employees closed assignments is enabled.
        /// </summary>
        public bool EnableDepartmentEditEmployeesClosedAssignments { get; set; }

        /// <summary>
        /// Gets or sets whether exempt requests/jobs from hours rules is enabled.
        /// </summary>
        public bool EnableExemptRequestsJobsFromHoursRules { get; set; }

        /// <summary>
        /// Gets or sets whether general policy management is enabled.
        /// </summary>
        public bool EnableGeneralPolicy { get; set; }
        /// <summary>
        /// Gets or sets whether tenure policy management is enabled.
        /// </summary>
        public bool EnableTenurePolicy { get; set; }

        /// <summary>
        /// When true, employees in this department can see the location map.
        /// </summary>
        public bool EnableEmployeeLocation { get; set; }

        /// <summary>
        /// Address or descriptive label for the department's home location (selected via geocoder).
        /// </summary>
        public string? DepartmentHomeLocation { get; set; }

        /// <summary>
        /// Latitude of the department's home location for default map centering.
        /// </summary>
        public decimal? DepartmentHomeLatitude { get; set; }

        /// <summary>
        /// Longitude of the department's home location for default map centering.
        /// </summary>
        public decimal? DepartmentHomeLongitude { get; set; }

        /// <summary>
        /// When true, department policy shows Restrict Web Login For Non-Admin Employee.
        /// </summary>
        public bool ShowRestrictWebLoginForNormalEmployee { get; set; } = false;

        /// <summary>
        /// When true, a normal employee in this department can have only one active login.
        /// </summary>
        public bool EnableNormalEmployeeOneSession { get; set; } = false;
    }
}

