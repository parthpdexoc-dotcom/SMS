using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class AdminValidator : BaseValidator<AdminDto>
    {
        public AdminValidator()
        {
            RuleFor(x => x.AdminTypeID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_ADMIN_TYPE_ID);

            RuleFor(x => x.FirstName)
                .MaximumLength(255)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_FIRST_NAME);

            RuleFor(x => x.LastName)
                .MaximumLength(255)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_LAST_NAME);

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_USERNAME)
                .MaximumLength(255)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_USERNAME);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PASSWORD)
                .MinimumLength(6)
                .WithMessage(ModelValidationErrorCodes.MINIMUM_LENGTH_PASSWORD)
                .MaximumLength(100)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_PASSWORD);

            RuleFor(x => x.Status)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_STATUS);
        }
    }
}
