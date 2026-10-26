using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeePositionValidator : AbstractValidator<EmployeePositionDto>
    {
        public EmployeePositionValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_POSITION_TITLE)
                .When(x => !string.IsNullOrEmpty(x.Title));
        }
    }
}

