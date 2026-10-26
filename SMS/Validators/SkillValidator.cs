using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class SkillValidator : BaseValidator<SkillDto>
    {
        public SkillValidator()
        {
            RuleFor(x => x.SkillShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_SKILL_SHORT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_SKILL_SHORT);

            RuleFor(x => x.SkillLong)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_SKILL_LONG);
        }
    }
}

