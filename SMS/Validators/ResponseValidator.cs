using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for ResponseDto using FluentValidation.
    /// </summary>
    public class ResponseValidator : AbstractValidator<ResponseDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseValidator"/> class.
        /// </summary>
        public ResponseValidator()
        {
            // SubmitterEmail is optional (auto-populated from employee token)
            // Only validate format and length if provided
            RuleFor(x => x.SubmitterEmail)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_RESPONSE_SUBMITTER_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.SubmitterEmail));

            RuleFor(x => x.SubmitterEmail)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_RESPONSE_SUBMITTER_EMAIL)
                .When(x => !string.IsNullOrEmpty(x.SubmitterEmail));

            // QuestionnaireId is optional - no validation required

            // SubmittedOn is optional - no validation required

            // PostPositionId is optional - no validation required

            // CreatedUserID (SubmittedBy) is handled by BaseEntity/BaseDto
        }
    }
}
