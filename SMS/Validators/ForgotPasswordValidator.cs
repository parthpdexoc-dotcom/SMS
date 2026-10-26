using FluentValidation;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Validators
{
    public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordValidator()
        {

            RuleFor(x => x.Otp)
                .MaximumLength(10).WithMessage("OTP cannot exceed 10 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Otp));

            RuleFor(x => x.NewPassword)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.NewPassword));

        }
    }
}
