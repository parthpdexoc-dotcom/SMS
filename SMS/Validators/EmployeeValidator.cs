using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeValidator()
        {
            RuleFor(x => x.DepartmentID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_DEPARTMENT_ID);

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_USERNAME)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_USERNAME);

            RuleFor(x => x.Password)
                .MinimumLength(6)
                .WithMessage(ModelValidationErrorCodes.MINIMUM_LENGTH_PASSWORD)
                .MaximumLength(100)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_PASSWORD)
                // Only run this validation if Password has a value. 
                // If it's null or "", this rule is skipped, which is what you want for updates.
                .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.BadgeNumber)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_BADGE_NUMBER)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_BADGE_NUMBER);

            RuleFor(x => x.EmployeeNumber)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_NUMBER)
                .When(x => !string.IsNullOrEmpty(x.EmployeeNumber));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_FIRST_NAME)
                .MaximumLength(100).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_FIRST_NAME);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_LAST_NAME)
                .MaximumLength(100).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_LAST_NAME);

            RuleFor(x => x.PrimaryPhone)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_PHONE)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT);

            RuleFor(x => x.SecondaryPhone)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.SecondaryPhone));

            RuleFor(x => x.PrimaryEmail)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_EMAIL)
                .MaximumLength(150).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_PRIMARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT);

            RuleFor(x => x.SecondaryEmail)
                .MaximumLength(150).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.SecondaryEmail));

            RuleFor(x => x.OffDutyCallBackPhoneNumber)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.OffDutyCallBackPhoneNumber));

            RuleFor(x => x.SmsTelephoneNumber)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_SMS_TELEPHONE_NUMBER)
                .When(x => x.HasSmsConsent);

            RuleFor(x => x.SmsTelephoneNumber)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_PHONE)
                .Must(PhoneNumberHelper.HasValidE164)
                .WithMessage(ModelValidationErrorCodes.INVALID_EMPLOYEE_SMS_TELEPHONE_NUMBER)
                .When(x => !string.IsNullOrEmpty(x.SmsTelephoneNumber));

            RuleFor(x => x.OffDutyCallBackEmail)
                .MaximumLength(150).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_SECONDARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.OffDutyCallBackEmail));

            RuleFor(x => x.CreatedBy)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_CREATED_BY)
                .When(x => !string.IsNullOrEmpty(x.CreatedBy));

            RuleFor(x => x.EditedBy)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_EDITED_BY)
                .When(x => !string.IsNullOrEmpty(x.EditedBy));
        }
    }
}

