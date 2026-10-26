using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequesterManagerValidator : BaseValidator<RequesterManagerDto>
    {
        public RequesterManagerValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_ID);

            RuleFor(x => x.EmployeeID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID);
        }
    }
}