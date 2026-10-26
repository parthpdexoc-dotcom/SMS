using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentValidator()
        {
            RuleFor(x => x.DepartmentCode)
                .MaximumLength(15).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_CODE)
                .When(x => !string.IsNullOrEmpty(x.DepartmentCode));

            RuleFor(x => x.DepartmentName)
                .MaximumLength(75).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_NAME)
                .When(x => !string.IsNullOrEmpty(x.DepartmentName));

            RuleFor(x => x.Address)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_ADDRESS)
                .When(x => !string.IsNullOrEmpty(x.Address));

            RuleFor(x => x.City)
                .MaximumLength(40).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_CITY)
                .When(x => !string.IsNullOrEmpty(x.City));

            RuleFor(x => x.State)
                .MaximumLength(30).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_STATE)
                .When(x => !string.IsNullOrEmpty(x.State));

            RuleFor(x => x.Zip)
                .MaximumLength(12).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_ZIP)
                .When(x => !string.IsNullOrEmpty(x.Zip));

            RuleFor(x => x.FirstName)
                .MaximumLength(15).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_FIRST_NAME)
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(20).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_LAST_NAME)
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Phone)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Fax)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_FAX)
                .When(x => !string.IsNullOrEmpty(x.Fax));

            RuleFor(x => x.Email)
                .MaximumLength(70).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Domain)
                .MaximumLength(60).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_DOMAIN)
                .When(x => !string.IsNullOrEmpty(x.Domain));
        }
    }
}

