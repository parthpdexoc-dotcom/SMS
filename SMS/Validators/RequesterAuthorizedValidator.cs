using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequesterAuthorizedValidator : BaseValidator<RequesterAuthorizedDto>
    {
        public RequesterAuthorizedValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_ID);

            RuleFor(x => x.FirstName)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_AUTHORIZED_FIRST_NAME)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_FIRST_NAME);

            RuleFor(x => x.LastName)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_AUTHORIZED_LAST_NAME)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_LAST_NAME);

            RuleFor(x => x.PrimaryPhone)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_AUTHORIZED_PRIMARY_PHONE)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PRIMARY_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT);

            RuleFor(x => x.SecondaryPhone)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SECONDARY_PHONE)
                .Must(PhoneNumberHelper.HasValidPhoneDigits)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PHONE_FORMAT)
                .When(x => x.SecondaryPhone != null);

            RuleFor(x => x.PrimaryEmail)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_AUTHORIZED_PRIMARY_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_PRIMARY_EMAIL);

            RuleFor(x => x.SecondaryEmail)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_AUTHORIZED_SECONDARY_EMAIL)
                .When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));

            // Validate that primary phone and secondary phone are not the same
            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.PrimaryPhone) || 
                           string.IsNullOrWhiteSpace(x.SecondaryPhone) || 
                           x.PrimaryPhone != x.SecondaryPhone)
                .WithMessage(ModelValidationErrorCodes.PRIMARY_AND_SECONDARY_PHONE_CANNOT_BE_SAME)
                .WithName("SecondaryPhone");

            // Validate that primary email and secondary email are not the same
            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.PrimaryEmail) || 
                           string.IsNullOrWhiteSpace(x.SecondaryEmail) || 
                           x.PrimaryEmail != x.SecondaryEmail)
                .WithMessage(ModelValidationErrorCodes.PRIMARY_AND_SECONDARY_EMAIL_CANNOT_BE_SAME)
                .WithName("SecondaryEmail")
                .When(x => !string.IsNullOrWhiteSpace(x.SecondaryEmail));
        }
    }
}