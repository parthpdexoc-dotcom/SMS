using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentDivisionsValidator : BaseValidator<DepartmentDivisionsDto>
    {
        public DepartmentDivisionsValidator()
        {
            RuleFor(x => x.DivisionShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_DIVISION_SHORT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_SHORT);

            RuleFor(x => x.DivisionLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_DIVISION_LONG)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DIVISION_LONG);
        }
    }
}

