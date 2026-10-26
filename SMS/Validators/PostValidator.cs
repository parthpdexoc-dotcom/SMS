using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PostValidator : AbstractValidator<PostDto>
    {
        public PostValidator()
        {
            RuleFor(x => x.PostTitle)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_TITLE)
                .MaximumLength(60)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_TITLE);

            RuleFor(x => x.PostDescription)
                .MaximumLength(1000)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DESCRIPTION)
                .When(x => !string.IsNullOrEmpty(x.PostDescription));

            RuleFor(x => x.TotalPostPositions)
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_TOTAL_JOB_POSITIONS);

            RuleFor(x => x.EmployeeTypeID)
                .Must(x => x.HasValue && x.Value != Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_TYPE_ID);

            RuleFor(x => x.PostStartTime)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_START_TIME)
                .Must(time => IsValid24HourTime(time))
                .WithMessage(ModelValidationErrorCodes.INVALID_JOB_START_TIME_FORMAT);

            RuleFor(x => x.PostEndTime)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_END_TIME)
                .Must(time => IsValid24HourTime(time))
                .WithMessage(ModelValidationErrorCodes.INVALID_JOB_END_TIME_FORMAT);

            RuleFor(x => x.PostLength)
                .NotNull()
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_JOB_LENGTH)
                .LessThanOrEqualTo(1440) // 1440 minutes = 24 hours
                .WithMessage(ModelValidationErrorCodes.JOB_LENGTH_EXCEEDS_24_HOURS)
                .When(x => x.PostLength>0);

            // 🔹 Notes & Location
            RuleFor(x => x.PostNotes)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_NOTES)
                .When(x => !string.IsNullOrEmpty(x.PostNotes));

            RuleFor(x => x.PostLocation)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_LOCATION)
                .When(x => !string.IsNullOrEmpty(x.PostLocation));

            // 🔹 Project/Task/Award/Fund/Organization (PTAEO-style fields)
            RuleFor(x => x.ProjectNumber)
                .MaximumLength(50)
                .WithMessage("Project number must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.ProjectNumber));

            RuleFor(x => x.TaskNumber)
                .MaximumLength(100)
                .WithMessage("Task number must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.TaskNumber));

            RuleFor(x => x.AwardNumber)
                .MaximumLength(50)
                .WithMessage("Award number must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.AwardNumber));

            RuleFor(x => x.FundNumber)
                .MaximumLength(50)
                .WithMessage("Fund number must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.FundNumber));

            RuleFor(x => x.OrganizationNumber)
                .MaximumLength(50)
                .WithMessage("Organization number must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.OrganizationNumber));

            // 🔹 Post contact (mirrors Request)
            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_FIRST_NAME)
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_LAST_NAME)
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.PrimaryPhone)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_PRIMARY_PHONE)
                .Must(SMS.Generic.Common.Helpers.PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.PrimaryPhone));

            RuleFor(x => x.CellPhone)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_CELL_PHONE)
                .Must(SMS.Generic.Common.Helpers.PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.CellPhone));

            RuleFor(x => x.PrimaryEmail)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_PRIMARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_REQUEST_PRIMARY_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.PrimaryEmail));

            // 🔹 Department-defined values
            RuleFor(x => x.PostDepartmentDefinedValue1)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedValue1));

            RuleFor(x => x.PostDepartmentDefinedValue2)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedValue2));

            RuleFor(x => x.PostDepartmentDefinedValue3)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedValue3));

            RuleFor(x => x.PostDepartmentDefinedValue4)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedValue4));

            RuleFor(x => x.PostDepartmentDefinedValue5)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedValue5));

            // 🔹 Money validations
            RuleFor(x => x.PostPayRate)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_JOB_PAY_RATE)
                .When(x => x.PostPayRate>0);

            RuleFor(x => x.PostPremiumRate)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_JOB_PREMIUM_RATE)
                .When(x => x.PostPremiumRate>0);

            // ===== SKILL VALIDATION =====
            When(x => x.IsSkillsRequired == true, () =>
            {
                RuleFor(x => x.SkillIDs)
                    .Must(list =>
                        list != null &&
                        list.Any() &&
                        list.All(id => id != Guid.Empty))
                    .WithMessage("At least one valid SkillID is required when IsSkillRequired is true.");
            });

            When(x => x.IsSkillsRequired == false || x.IsSkillsRequired == null, () =>
            {
                RuleFor(x => x.SkillIDs)
                    .Null()
                    .WithMessage("SkillIDs must be null when IsSkillRequired is not true.");
            });
        }

        /// <summary>
        /// Validates that the TimeOnly value is in valid 24-hour format.
        /// TimeOnly already handles 24-hour format, but this ensures the time is within valid range.
        /// </summary>
        private bool IsValid24HourTime(TimeOnly time)
        {
            // TimeOnly already represents time in 24-hour format (00:00:00 to 23:59:59)
            // This validation ensures the time is within the valid 24-hour range
            return time.Hour >= 0 && time.Hour < 24 && 
                   time.Minute >= 0 && time.Minute < 60 &&
                   time.Second >= 0 && time.Second < 60;
        }
    }
}

