using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;
using System;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for QuestionDto using FluentValidation.
    /// </summary>
    public class QuestionValidator : AbstractValidator<QuestionDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionValidator"/> class.
        /// </summary>
        public QuestionValidator()
        {
            // QuestionText must not be empty
            RuleFor(x => x.QuestionText)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTION_QUESTION_TEXT);
                
            // QuestionnaireId must not be empty
            RuleFor(x => x.QuestionnaireId)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTION_QUESTIONNAIRE_ID);

            // DisplayOrder must be greater than or equal to 1
            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(1).WithMessage(ModelValidationErrorCodes.INVALID_QUESTION_DISPLAY_ORDER);

            // QuestionType must be a valid enum
            RuleFor(x => x.QuestionType)
                .IsInEnum().WithMessage(ModelValidationErrorCodes.INVALID_QUESTION_QUESTION_TYPE);

            // QuestionId must not be empty on update scenarios (when QuestionId is provided)
            RuleFor(x => x.QuestionId)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTION_QUESTION_ID)
                .NotEqual(Guid.Empty).WithMessage(ModelValidationErrorCodes.REQUIRED_QUESTION_QUESTION_ID)
                .When(x => x.QuestionId.HasValue);
        }
    }
}
