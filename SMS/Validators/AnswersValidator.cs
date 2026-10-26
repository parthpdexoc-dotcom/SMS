using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using System.Text.Json;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for AnswersDto using FluentValidation.
    /// </summary>
    public class AnswersValidator : AbstractValidator<AnswersDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswersValidator"/> class.
        /// </summary>
        public AnswersValidator()
        {
            // ResponseId is required and must not be Guid.Empty
            RuleFor(x => x.ResponseId)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_ANSWER_RESPONSE_ID)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.INVALID_ANSWER_RESPONSE_ID);

            // QuestionId is required and must not be Guid.Empty
            RuleFor(x => x.QuestionId)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_ANSWER_QUESTION_ID)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.INVALID_ANSWER_QUESTION_ID);

            // Answer is required and must be valid JSON
            RuleFor(x => x.Answer)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_ANSWER_ANSWER)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_ANSWER_ANSWER)
                .WithMessage(ModelValidationErrorCodes.INVALID_ANSWER_JSON_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.Answer));
        }
    }
}
