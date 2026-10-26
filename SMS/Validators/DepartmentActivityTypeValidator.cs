using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentActivityTypeValidator : BaseValidator<DepartmentActivityTypeDto>
    {
        public DepartmentActivityTypeValidator()
        {
            RuleFor(x => x.ActivityTypeShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_ACTIVITY_TYPE_SHORT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ACTIVITY_TYPE_SHORT);

            RuleFor(x => x.ActivityTypeLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_ACTIVITY_TYPE_LONG)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ACTIVITY_TYPE_LONG);
        }
    }
}

