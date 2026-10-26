using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for DepartmentAdmin DTO operations.
    /// </summary>
    public class DepartmentAdminValidator : BaseValidator<DepartmentAdminDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentAdminValidator"/> class.
        /// </summary>
        public DepartmentAdminValidator()
        {
            RuleFor(x => x.EmployeeID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID);

            RuleFor(x => x.AdminTypeID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_ADMIN_TYPE_ID);

            // DepartmentID is populated from token by the service (IDepartmentScoped).
            // Audit fields (CreatedDate, CreatedUserID, CreatedBy, EditedDate, EditedUserID, EditedBy) come from BaseDto and are set by the service.
        }
    }
}
