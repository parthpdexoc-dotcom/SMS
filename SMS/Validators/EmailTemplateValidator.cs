using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for EmailTemplateDto using FluentValidation.
    /// </summary>
    public class EmailTemplateValidator : AbstractValidator<EmailTemplateDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateValidator"/> class.
        /// </summary>
        public EmailTemplateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_EMAIL_TEMPLATE_TITLE)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMAIL_TEMPLATE_TITLE);

            RuleFor(x => x.Subject)
                .MaximumLength(500)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EMAIL_TEMPLATE_SUBJECT)
                .When(x => !string.IsNullOrEmpty(x.Subject));
        }
    }
}
