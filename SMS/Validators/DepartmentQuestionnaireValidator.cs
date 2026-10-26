using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for DepartmentQuestionnaireDto.
    /// </summary>
    public class DepartmentQuestionnaireValidator : AbstractValidator<DepartmentQuestionnaireDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentQuestionnaireValidator"/> class.
        /// </summary>
        public DepartmentQuestionnaireValidator()
        {
           
            RuleFor(x => x.QuestionnaireId)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_QUESTIONNAIRE_QUESTIONNAIRE_ID);
        }
    }
}
