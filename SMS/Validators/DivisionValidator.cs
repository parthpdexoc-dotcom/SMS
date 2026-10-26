using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DivisionValidator : AbstractValidator<DivisionDto>
    {
        public DivisionValidator()
        {
            RuleFor(x => x.DivisionName)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_NAME)
                .When(x => !string.IsNullOrEmpty(x.DivisionName));

            RuleFor(x => x.DivisionDescription)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_DESCRIPTION)
                .When(x => !string.IsNullOrEmpty(x.DivisionDescription));

            RuleFor(x => x.Contact)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_CONTACT)
                .When(x => !string.IsNullOrEmpty(x.Contact));

            RuleFor(x => x.Phone)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.PvtLine)
                .MaximumLength(15).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_PVT_LINE)
                .When(x => !string.IsNullOrEmpty(x.PvtLine));

            RuleFor(x => x.Email)
                .MaximumLength(35).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.NmBilling)
                .MaximumLength(20).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_NM_BILLING)
                .When(x => !string.IsNullOrEmpty(x.NmBilling));
        }
    }
}

