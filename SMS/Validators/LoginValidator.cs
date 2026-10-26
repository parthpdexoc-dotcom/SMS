using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class LoginValidator : AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_USERNAME)
                .MaximumLength(25).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_USERNAME);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_PASSWORD)
                .MinimumLength(6).WithMessage(ModelValidationErrorCodes.MINIMUM_LENGTH_PASSWORD)
                .MaximumLength(100).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_PASSWORD);
        }
    }
}

