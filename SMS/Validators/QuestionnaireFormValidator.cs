using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validates activity form POST bodies so nested <see cref="QuestionnaireDto"/> rules run
    /// (e.g. description max length) — avoids 500 truncation from SQL when limits are exceeded.
    /// </summary>
    public class QuestionnaireFormValidator : AbstractValidator<QuestionnaireFormDto>
    {
        public QuestionnaireFormValidator()
        {
            RuleFor(x => x.Questionnaire)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage(nameof(ModelValidationErrorCodes.QUESTIONNAIRE_CANNOT_BE_NULL))
                .SetValidator(new QuestionnaireValidator());
        }
    }
}
