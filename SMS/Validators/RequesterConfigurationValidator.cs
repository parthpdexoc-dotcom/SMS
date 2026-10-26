using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequesterConfigurationValidator : AbstractValidator<RequesterConfigurationDto>
    {
        public RequesterConfigurationValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_ID);

            RuleFor(x => x.Open24Hr)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_OPEN24_HR);

            RuleFor(x => x.Sba24Hr)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_SBA24_HR);

            RuleFor(x => x.HourPerDay)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_HOUR_PER_DAY);

            RuleFor(x => x.HourPerWeek)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_HOUR_PER_WEEK);

            RuleFor(x => x.SupervisorOverride)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_SUPERVISOR_OVERRIDE);

            RuleFor(x => x.RequesterDefault)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_REQUESTER_DEFAULT);

            RuleFor(x => x.DepartmentID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIGURATION_DEPARTMENT_ID);
        }
    }
}

