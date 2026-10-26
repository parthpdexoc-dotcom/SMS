using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeeStatusValidator : BaseValidator<EmployeeStatusDto>
    {
        public EmployeeStatusValidator()
        {
            RuleFor(x => x.StatusShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_STATUS_SHORT)
                .MaximumLength(100).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_STATUS_SHORT);

            RuleFor(x => x.StatusLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_STATUS_LONG)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_STATUS_LONG);
        }
    }
}

