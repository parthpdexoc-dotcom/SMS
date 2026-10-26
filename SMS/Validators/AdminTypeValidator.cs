using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class AdminTypeValidator : BaseValidator<AdminTypeDto>
    {
        public AdminTypeValidator()
        {
            RuleFor(x => x.AdminTypeShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_ADMIN_TYPE_SHORT)
                .MaximumLength(100).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ADMIN_TYPE_SHORT);

            RuleFor(x => x.AdminTypeLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_ADMIN_TYPE_LONG)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ADMIN_TYPE_LONG);

        }
    }
}
