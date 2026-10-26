using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeeAdminValidator : AbstractValidator<EmployeeAdminDto>
    {
        public EmployeeAdminValidator()
        {
            RuleFor(x => x.AdminTypeID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ADMIN_ADMIN_TYPE_ID);

            RuleFor(x => x.EmployeeID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ADMIN_EMPLOYEE_ID);

            RuleFor(x => x.CreatedBy)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_ADMIN_CREATED_BY)
                .When(x => !string.IsNullOrEmpty(x.CreatedBy));

            RuleFor(x => x.EditedBy)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMPLOYEE_ADMIN_EDITED_BY)
                .When(x => !string.IsNullOrEmpty(x.EditedBy));
        }
    }
}

