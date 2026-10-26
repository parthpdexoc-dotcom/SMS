using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for InvoiceDto using FluentValidation.
    /// </summary>
    public class InvoiceValidator : AbstractValidator<InvoiceDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceValidator"/> class.
        /// </summary>
        public InvoiceValidator()
        {
            RuleFor(x => x.InvoiceNo)
                .MaximumLength(30)
                .WithMessage("InvoiceNo cannot exceed 30 characters")
                .When(x => !string.IsNullOrEmpty(x.InvoiceNo));

            RuleFor(x => x.BillingFromName)
                .MaximumLength(200)
                .WithMessage("BillingFromName cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingFromName));

            RuleFor(x => x.BillingFromAddress)
                .MaximumLength(500)
                .WithMessage("BillingFromAddress cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingFromAddress));

            RuleFor(x => x.BillingFromPhoneNumber)
                .MaximumLength(50)
                .WithMessage("BillingFromPhoneNumber cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingFromPhoneNumber));

            RuleFor(x => x.BillingFromEmail)
                .MaximumLength(100)
                .WithMessage("BillingFromEmail cannot exceed 100 characters")
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.BillingFromEmail));

            RuleFor(x => x.BillingFromContact)
                .MaximumLength(100)
                .WithMessage("BillingFromContact cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingFromContact));

            RuleFor(x => x.BillingToName)
                .MaximumLength(200)
                .WithMessage("BillingToName cannot exceed 200 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingToName));

            RuleFor(x => x.BillingToAddress)
                .MaximumLength(500)
                .WithMessage("BillingToAddress cannot exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingToAddress));

            RuleFor(x => x.BillingToPhoneNumber)
                .MaximumLength(50)
                .WithMessage("BillingToPhoneNumber cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingToPhoneNumber));

            RuleFor(x => x.BillingToEmail)
                .MaximumLength(100)
                .WithMessage("BillingToEmail cannot exceed 100 characters")
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_DIVISION_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.BillingToEmail));

            RuleFor(x => x.BillingToContact)
                .MaximumLength(100)
                .WithMessage("BillingToContact cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.BillingToContact));
        }
    }
}
