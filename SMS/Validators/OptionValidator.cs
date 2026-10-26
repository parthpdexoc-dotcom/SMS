using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for OptionDto.
    /// </summary>
    public class OptionValidator : AbstractValidator<OptionDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionValidator"/> class.
        /// </summary>
        public OptionValidator()
        {
            RuleFor(x => x.QuestionId)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTION_ID);

            RuleFor(x => x.OptionText)
                .MaximumLength(500)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_OPTION_TEXT)
                .When(x => !string.IsNullOrEmpty(x.OptionText));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(1)
                .WithMessage(ModelValidationErrorCodes.INVALID_OPTION_DISPLAY_ORDER);
        }
    }
}
