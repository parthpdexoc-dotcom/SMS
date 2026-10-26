using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class QuestionnaireValidator : AbstractValidator<QuestionnaireDto>
    {
        public QuestionnaireValidator()
        {
            RuleFor(x => x.Title)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTIONNAIRE_TITLE)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTIONNAIRE_TITLE)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_QUESTIONNAIRE_TITLE);

            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_QUESTIONNAIRE_DESCRIPTION)
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTIONNAIRE_IS_ACTIVE);

            RuleFor(x => x.QuestionnaireType)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTIONNAIRE_TYPE);
        }
    }
}
