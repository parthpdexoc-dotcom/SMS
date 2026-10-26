using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Helpers;

namespace SMS.Validators
{
    public class AdminForgotPasswordValidator : AbstractValidator<AdminForgotPasswordDto>
    {
        public AdminForgotPasswordValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_USERNAME)
                .MaximumLength(25)
                .WithMessage(ValidationMessageHelper.GetMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_USERNAME,25));

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_NEW_PASSWORD)
                .MinimumLength(6)
                .WithMessage(ModelValidationErrorCodes.MINIMUM_LENGTH_NEW_PASSWORD);
        }
    }
}
