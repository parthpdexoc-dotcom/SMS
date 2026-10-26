using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeeTenureValidator : BaseValidator<EmployeeTenureDto>
    {
        public EmployeeTenureValidator()
        {
            RuleFor(x => x.DepartmentID)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_DEPARTMENT_ID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_DEPARTMENT_ID)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_DEPARTMENT_ID);

            RuleFor(x => x.EmployeeID)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID);

            RuleFor(x => x.EmployeeTypeID)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_TYPE_ID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_TYPE_ID)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_TYPE_ID);           

            RuleFor(x => x)
                .Must(x => !x.DateOfPromotion.HasValue || !x.DateOfHire.HasValue || 
                          x.DateOfPromotion.Value >= x.DateOfHire.Value)
                .WithMessage(ModelValidationErrorCodes.PROMOTION_DATE_MUST_BE_GREATER_OR_EQUAL_TO_HIRE_DATE)
                .WithName("DateOfPromotion");
        }
    }
}

