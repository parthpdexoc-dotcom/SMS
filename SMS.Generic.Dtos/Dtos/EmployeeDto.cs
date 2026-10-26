using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Entities.Entities;
using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Employee operations.
    /// </summary>
    public class EmployeeDto : BaseDto
    {
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public EmployeeDto() { }

        /// <summary>
        /// Copy constructor. Clones HR-updatable properties from the source except Password
        /// and user-definable settings (notification bits, off-duty callback, subsidized vehicle).
        /// New employees keep DTO defaults for those settings.
        /// </summary>
        /// <param name="source">The employee DTO to clone from.</param>
        public void CloneEmployee(EmployeeDto source)
        {
            if (source == null) return;

            EmployeeID = source.EmployeeID ?? EmployeeID;
            EmployeeCode = source.EmployeeCode;
            DepartmentID = source.DepartmentID;
            EmployeeTypeID = source.EmployeeTypeID;
            StatusID = source.StatusID;
            IsApproved = source.IsApproved;
            FirstName = source.FirstName;
            LastName = source.LastName;
            PrimaryPhone = source.PrimaryPhone;
            SecondaryPhone = source.SecondaryPhone;
            PrimaryEmail = source.PrimaryEmail;
            SecondaryEmail = source.SecondaryEmail;
            Otp = source.Otp;
            IsPasswordReset = source.IsPasswordReset;
            HourlySalary = source.HourlySalary;
            HrCostCenterCode = source.HrCostCenterCode;
            DivisionID = source.DivisionID;
            EmployeeDepartmentDefinedField1 = source.EmployeeDepartmentDefinedField1;
            EmployeeDepartmentDefinedField2 = source.EmployeeDepartmentDefinedField2;
            EmployeeDepartmentDefinedField3 = source.EmployeeDepartmentDefinedField3;
            EmployeeDepartmentDefinedField4 = source.EmployeeDepartmentDefinedField4;
            ImportedDateOfHire = source.ImportedDateOfHire;
            ImportedDateOfPromotion = source.ImportedDateOfPromotion;
            EmployeeType = source.EmployeeType;
            Status = source.Status;
        }

        /// <summary>
        /// Calls <see cref="CloneEmployee"/> then copies identity/login fields and user-definable settings
        /// that <see cref="CloneEmployee"/> omits, for use as a baseline before applying JSON Patch.
        /// </summary>
        /// <param name="source">The employee DTO loaded from the API (e.g. GET by id).</param>
        public void CloneEmployeeForPatchMerge(EmployeeDto source)
        {
            if (source == null) return;

            CloneEmployee(source);

            EmployeeNumber = source.EmployeeNumber;
            Username = source.Username;
            BadgeNumber = source.BadgeNumber;
            Password = source.Password;
            IsSubsidizedVehicle = source.IsSubsidizedVehicle;
            OffDutyCallBackPhoneNumber = source.OffDutyCallBackPhoneNumber;
            OffDutyCallBackEmail = source.OffDutyCallBackEmail;
            SmsTelephoneNumber = source.SmsTelephoneNumber;
            HasSmsConsent = source.HasSmsConsent;
            NotifyWhenJobsPosted = source.NotifyWhenJobsPosted;
            NotifyJobStartingIn24Hours = source.NotifyJobStartingIn24Hours;
            NotifyJobStartingIn1Hour = source.NotifyJobStartingIn1Hour;
        }

        public bool HasChanges(EmployeeDto source)
        {
            if (source == null) return false;

            return !string.Equals(EmployeeNumber, source.EmployeeNumber, StringComparison.Ordinal)
                || !string.Equals(EmployeeCode, source.EmployeeCode, StringComparison.Ordinal)
                || DepartmentID != source.DepartmentID
                || EmployeeTypeID != source.EmployeeTypeID
                || StatusID != source.StatusID
                || IsApproved != source.IsApproved
                || !string.Equals(BadgeNumber, source.BadgeNumber, StringComparison.Ordinal)
                || !string.Equals(FirstName, source.FirstName, StringComparison.Ordinal)
                || !string.Equals(LastName, source.LastName, StringComparison.Ordinal)
                || !string.Equals(Username, source.Username, StringComparison.Ordinal)
                || !string.Equals(PrimaryPhone, source.PrimaryPhone, StringComparison.Ordinal)
                || !string.Equals(SecondaryPhone, source.SecondaryPhone, StringComparison.Ordinal)
                || !string.Equals(PrimaryEmail, source.PrimaryEmail, StringComparison.Ordinal)
                || !string.Equals(SecondaryEmail, source.SecondaryEmail, StringComparison.Ordinal)
                || !string.Equals(Otp, source.Otp, StringComparison.Ordinal)
                || IsPasswordReset != source.IsPasswordReset
                || HourlySalary != source.HourlySalary
                || !string.Equals(HrCostCenterCode, source.HrCostCenterCode, StringComparison.Ordinal)
                || DivisionID != source.DivisionID
                || !string.Equals(EmployeeDepartmentDefinedField1, source.EmployeeDepartmentDefinedField1, StringComparison.Ordinal)
                || !string.Equals(EmployeeDepartmentDefinedField2, source.EmployeeDepartmentDefinedField2, StringComparison.Ordinal)
                || !string.Equals(EmployeeDepartmentDefinedField3, source.EmployeeDepartmentDefinedField3, StringComparison.Ordinal)
                || !string.Equals(EmployeeDepartmentDefinedField4, source.EmployeeDepartmentDefinedField4, StringComparison.Ordinal)
                || ImportedDateOfHire != source.ImportedDateOfHire
                || ImportedDateOfPromotion != source.ImportedDateOfPromotion;
        }

        /// <summary>
        /// Primary Key - Employee ID (GUID)
        /// </summary>
        [Newtonsoft.Json.JsonProperty("employeeID")]
        [System.Text.Json.Serialization.JsonPropertyName("employeeID")]
        public Guid? EmployeeID { get; set; }
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Human-readable employee code from sequence (e.g. 000001).
        /// </summary>
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Department Type foreign key
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Employee Type foreign key
        /// </summary>
        public Guid? EmployeeTypeID { get; set; }

        /// <summary>
        /// Employee Status foreign key
        /// </summary>
        public Guid? StatusID { get; set; }

        /// <summary>
        /// Indicates whether the employee is approved.
        /// </summary>
        public bool IsApproved { get; set; } = true;

        /// <summary>
        /// Indicates whether the employee has a subsidized vehicle.
        /// </summary>
        public bool IsSubsidizedVehicle { get; set; }

        /// <summary>
        /// Employee badge number
        /// </summary>
        public string? BadgeNumber { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string? LastName { get; set; }

        public string GetFullName() => FormatDisplayName(LastName, FirstName);

        /// <summary>
        /// Username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password (BCrypt hashed)
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Primary phone
        /// </summary>
        public string? PrimaryPhone
        {
            get => _primaryPhone;
            set => _primaryPhone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Secondary phone
        /// </summary>
        public string? SecondaryPhone
        {
            get => _secondaryPhone;
            set => _secondaryPhone = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        private string? _primaryPhone;
        private string? _secondaryPhone;

        /// <summary>
        /// Primary email
        /// </summary>
        public string? PrimaryEmail { get; set; }

        /// <summary>
        /// Secondary email
        /// </summary>
        public string? SecondaryEmail { get; set; }

        /// <summary>
        /// Phone number shown under off-duty call back details (distinct from primary/secondary phone).
        /// </summary>
        public string? OffDutyCallBackPhoneNumber
        {
            get => _offDutyCallBackPhoneNumber;
            set => _offDutyCallBackPhoneNumber = PhoneNumberHelper.PhoneNumberFormatHelper(value);
        }

        /// <summary>
        /// Phone number used for SMS notifications.
        /// </summary>
        public string? SmsTelephoneNumber
        {
            get => _smsTelephoneNumber;
            set => _smsTelephoneNumber = PhoneNumberHelper.NormalizeE164(value);
        }

        /// <summary>
        /// When true, the employee has accepted the SMS consent disclosure.
        /// </summary>
        public bool HasSmsConsent { get; set; } = false;

        /// <summary>
        /// Email shown under off-duty call back details (distinct from primary/secondary email).
        /// </summary>
        public string? OffDutyCallBackEmail { get; set; }

        /// <summary>
        /// Notification preference: email when new jobs are posted/released. False = no notification.
        /// </summary>
        public bool NotifyWhenJobsPosted { get; set; } = false;

        /// <summary>
        /// Notification preference: email reminder 24 hours before a job starts. False = no notification.
        /// </summary>
        public bool NotifyJobStartingIn24Hours { get; set; } = false;

        /// <summary>
        /// Notification preference: email reminder 1 hour before a job starts. False = no notification.
        /// </summary>
        public bool NotifyJobStartingIn1Hour { get; set; } = false;

        private string? _offDutyCallBackPhoneNumber;
        private string? _smsTelephoneNumber;

        /// <summary>
        /// OTP used in forgot password flow.
        /// </summary>
        public string? Otp { get; set; }

        /// <summary>
        /// Indicates whether the user has completed a password reset flow.
        /// </summary>
        public bool IsPasswordReset { get; set; }

        /// <summary>
        /// Hourly salary
        /// </summary>
        public decimal? HourlySalary { get; set; }

        /// <summary>
        /// HR Cost Center Code
        /// </summary>
        public string? HrCostCenterCode { get; set; }

        /// <summary>
        /// Division foreign key
        /// </summary>
        public Guid? DivisionID { get; set; }

        /// <summary>
        /// Employee Department Defined Field 1
        /// </summary>
        public string? EmployeeDepartmentDefinedField1 { get; set; }

        /// <summary>
        /// Employee Department Defined Field 2
        /// </summary>
        public string? EmployeeDepartmentDefinedField2 { get; set; }

        /// <summary>
        /// Employee Department Defined Field 3
        /// </summary>
        public string? EmployeeDepartmentDefinedField3 { get; set; }

        /// <summary>
        /// Employee Department Defined Field 4
        /// </summary>
        public string? EmployeeDepartmentDefinedField4 { get; set; }

        /// <summary>
        /// Import-only hire date used by the background employee Excel job.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateOnly? ImportedDateOfHire { get; set; }

        /// <summary>
        /// Import-only promotion date used by the background employee Excel job.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public DateOnly? ImportedDateOfPromotion { get; set; }

        public List<SuspendedSkillPeriodDto> SuspendedSkillPeriods { get; set; } = new();

        /// <summary>
        /// Department used to resolve which skills count as "suspended": when <paramref name="tokenOrFilterDepartmentId"/> is set and non-empty, use it (e.g. token department); otherwise <see cref="DepartmentID"/>.
        /// </summary>
        public Guid GetDepartmentIdForSuspendedSkillLookup(Guid? tokenOrFilterDepartmentId)
        {
            if (tokenOrFilterDepartmentId.HasValue && tokenOrFilterDepartmentId.Value != Guid.Empty)
                return tokenOrFilterDepartmentId.Value;
            return DepartmentID;
        }

        /// <summary>
        /// Populates <see cref="SuspendedSkillPeriods"/> from <see cref="EmployeeSkills"/> for skills whose IDs are in <paramref name="suspendedSkillIdsInDepartment"/>.
        /// </summary>
        public void PopulateSuspendedSkillPeriods(IReadOnlySet<Guid> suspendedSkillIdsInDepartment)
        {
            ArgumentNullException.ThrowIfNull(suspendedSkillIdsInDepartment);

            if (EmployeeSkills is not { Count: > 0 })
            {
                SuspendedSkillPeriods = new List<SuspendedSkillPeriodDto>();
                return;
            }

            SuspendedSkillPeriods = EmployeeSkills
                .Where(es => suspendedSkillIdsInDepartment.Contains(es.SkillID))
                .Select(es => new SuspendedSkillPeriodDto { StartDate = es.StartDate, EndDate = es.EndDate })
                .ToList();
        }

        /// <summary>
        /// From assigned-family post positions (with date of work), returns those to cancel for suspension: still scheduled (not past start), not unclosed, and work date is in <see cref="SuspendedSkillPeriods"/>.
        /// </summary>
        /// <param name="isPostPositionDateTimePassed">True when the position’s start is at or before <paramref name="now"/> (supply from <c>JobDateHelper.IsPostPositionDateTimePassed</c> in the service layer).</param>
        public List<PostPosition> SelectPostPositionsToCancelForSuspendedSchedule(
            IEnumerable<PostPosition> assignedFamilyWithDateOfWork,
            DateTime now,
            Func<PostPosition, DateTime, bool> isPostPositionDateTimePassed)
        {
            ArgumentNullException.ThrowIfNull(assignedFamilyWithDateOfWork);
            ArgumentNullException.ThrowIfNull(isPostPositionDateTimePassed);

            var toCancel = new List<PostPosition>();
            foreach (var e in assignedFamilyWithDateOfWork)
            {
                if (e.PostPositionID == Guid.Empty || !e.DateOfWork.HasValue)
                    continue;
                if (e.PostPositionStatus == PostPositionStatus.Unclosed)
                    continue;
                if (isPostPositionDateTimePassed(e, now))
                    continue;
                if (!IsSuspendedOn(e.DateOfWork.Value))
                    continue;
                toCancel.Add(e);
            }

            return toCancel;
        }

        public bool IsSuspendedOn(DateOnly date)
        {
            if (SuspendedSkillPeriods == null || SuspendedSkillPeriods.Count == 0)
                return false;

            return SuspendedSkillPeriods.Any(x =>
                x.StartDate <= date &&
                (!x.EndDate.HasValue || x.EndDate.Value >= date));
        }

        public List<DateOnly> RemoveSuspendedDates(List<DateOnly> dates)
        {
            if (dates == null || dates.Count == 0)
                return new List<DateOnly>();

            return dates.Where(d => !IsSuspendedOn(d)).ToList();
        }

        public List<EmployeeSkillDto>? EmployeeSkills { get; set; }
        public EmployeeTypeDto? EmployeeType { get; set; }
        public EmployeeStatusDto? Status { get; set; }

        /// <summary>
        /// Profile photo MIME content type (e.g. image/png). Null when no profile photo is set or not loaded.
        /// </summary>
        public string? ProfilePhotoContentType { get; set; }

        /// <summary>
        /// Profile photo image bytes as base64. Use in img src as "data:{ProfilePhotoContentType};base64,{ProfilePhotoBase64}". Null when no photo.
        /// </summary>
        public string? ProfilePhotoBase64 { get; set; }
        public List<DateOnly> ValidateSkillsForDates(List<Guid> requiredSkillIds,IEnumerable<DateOnly> dates)
        {
            // ✅ No required skills → all dates valid
            if (requiredSkillIds == null || requiredSkillIds.Count == 0)
                return dates as List<DateOnly> ?? dates.ToList();

            var employeeSkills = EmployeeSkills;

            // ❌ No employee skills → no valid dates
            if (employeeSkills == null || employeeSkills.Count == 0)
                return new List<DateOnly>();

            // 🔥 Step 1: Group skills by SkillID
            var skillLookup = employeeSkills.GroupBy(s => s.SkillID).ToDictionary(g => g.Key, g => g.ToList());

            // 🔥 Step 2: Early exit → if any required skill doesn't exist at all
            foreach (var skillId in requiredSkillIds)
            {
                if (!skillLookup.ContainsKey(skillId))
                    return new List<DateOnly>();
            }
            var validDates = new List<DateOnly>();

            // 🔥 Step 3: Validate per date
            foreach (var date in dates)
            {
                bool isValid = true;

                foreach (var skillId in requiredSkillIds)
                {
                    var skills = skillLookup[skillId];

                    // Check if ANY record satisfies date condition
                    bool hasValidSkill = false;

                    for (int i = 0; i < skills.Count; i++)
                    {
                        var s = skills[i];

                        if (s.StartDate <= date &&
                            (s.EndDate == null || s.EndDate >= date))
                        {
                            hasValidSkill = true;
                            break;
                        }
                    }
                    if (!hasValidSkill)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    validDates.Add(date);
            }
            return validDates;
        }
    }
    /// <summary>
    /// Employee DTO with Status and Employee Type details
    /// </summary>
    public class EmployeeResponseDto : EmployeeDto
    {
        public string? Status { get; set; }
        public string? EmployeeType { get; set; }
    }

    public class SuspendedSkillPeriodDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

}