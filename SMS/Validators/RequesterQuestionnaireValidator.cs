using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for PostQuestionnaireDto.
    /// </summary>
    public class RequesterQuestionnaireValidator : AbstractValidator<RequesterQuestionnaireDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterQuestionnaireValidator"/> class.
        /// </summary>
        public RequesterQuestionnaireValidator()
        {
            RuleFor(x => x.RequesterId)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_ID);

            RuleFor(x => x.QuestionnaireId)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTIONNAIRE_ID);
        }
    }
}
