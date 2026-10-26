using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequesterValidator : AbstractValidator<RequesterDto>
    {
        public RequesterValidator()
        {
            RuleFor(x => x.DepartmentDefinedNumber)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_DEFINED_NUMBER)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_DEFINED_NUMBER)
                .When(x => x.DepartmentDefinedNumber != null);

            RuleFor(x => x.BusinessTypeID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_BUSINESS_TYPE_ID);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_NAME)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_NAME)
                .When(x => x.Name != null);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_ADDRESS)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_ADDRESS)
                .When(x => x.Address != null);

            RuleFor(x => x.City)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CITY)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_CITY)
                .When(x => x.City != null);

            RuleFor(x => x.State)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_STATE)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_STATE)
                .When(x => x.State != null);

            RuleFor(x => x.Zip)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_ZIP)
                .MaximumLength(10).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_ZIP)
                .When(x => x.Zip != null);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_EMAIL)
                .When(x => x.Email != null);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_PHONE)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => x.Phone != null);

            // Billing/Payment phone (InvoicePhone) should follow the same 10-digit format rule.
            RuleFor(x => x.InvoicePhone)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrWhiteSpace(x.InvoicePhone));
        }
    }
}

