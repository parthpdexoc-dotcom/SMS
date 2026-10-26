using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentConfigurationValidator : AbstractValidator<DepartmentConfigurationDto>
    {
        public DepartmentConfigurationValidator()
        {
            RuleFor(x => x.DepartmentID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_CONFIGURATION_DEPARTMENT_ID);

            RuleFor(x => x.DepartmentHomeLocation)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_CONFIGURATION_DEPARTMENT_HOME_LOCATION)
                .When(x => x.EnableEmployeeLocation == true);
        }
    }
}

