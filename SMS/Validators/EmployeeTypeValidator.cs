using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeeTypeValidator : BaseValidator<EmployeeTypeDto>
    {
        public EmployeeTypeValidator()
        {
            RuleFor(x => x.EmployeeTypeShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_TYPE_SHORT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_TYPE_SHORT);

            RuleFor(x => x.EmployeeTypeLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_TYPE_LONG)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_TYPE_LONG);
        }
    }
}

