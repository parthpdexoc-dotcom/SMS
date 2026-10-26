using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for SendMailRequestDto using FluentValidation.
    /// </summary>
    public class SendMailRequestDtoValidator : AbstractValidator<SendMailRequestDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendMailRequestDtoValidator"/> class.
        /// </summary>
        public SendMailRequestDtoValidator()
        {
            RuleFor(x => x.EmailType)
                .IsInEnum()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_SEND_MAIL_EMAIL_TYPE);

            RuleFor(x => x.Emails)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_SEND_MAIL_TO_EMAILS);

            RuleForEach(x => x.Emails)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_SEND_MAIL_TO_EMAILS)
                .Must(email => EmailAddressHelper.IsValidEmail(email))
                .WithMessage(ModelValidationErrorCodes.INVALID_SEND_MAIL_EMAIL_FORMAT)
                .MaximumLength(256)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_SEND_MAIL_EMAIL);

            RuleFor(x => x.FromEmail)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_SEND_MAIL_EMAIL_FORMAT)
                .MaximumLength(256)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_SEND_MAIL_EMAIL)
                .When(x => !string.IsNullOrWhiteSpace(x.FromEmail));
        }
    }
}
