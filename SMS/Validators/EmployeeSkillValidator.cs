using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EmployeeSkillValidator : AbstractValidator<EmployeeSkillDto>
    {
        public EmployeeSkillValidator()
        {
            // Currently we only have max length error codes defined for EmployeeSkill in ModelValidationErrorCodes.
            // If you later add Title/Description fields to EmployeeSkillDto, enforce their lengths using:
            // MAXIMUM_LENGTH_EMPLOYEE_SKILL_TITLE and MAXIMUM_LENGTH_EMPLOYEE_SKILL_DESCRIPTION.

            RuleFor(x => x.EmployeeID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EMPLOYEE_ID);

            RuleFor(x => x.SkillID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_SKILL_SHORT);
        }
    }
}

