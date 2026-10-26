using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequesterConfigValidator : AbstractValidator<RequesterConfigDto>
    {
        public RequesterConfigValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_CONFIG_REQUESTER_ID);

            RuleFor(x => x.DefinedSkills)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_CONFIG_DEFINED_SKILLS)
                .When(x => x.DefinedSkills != null);

            RuleFor(x => x.DefineRequesterManager)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUESTER_CONFIG_DEFINE_REQUESTER_MANAGER)
                .When(x => x.DefineRequesterManager != null);
        }
    }
}
