using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for <see cref="PaymentAllocationDto"/> using FluentValidation.
    /// </summary>
    public class PaymentAllocationValidator : BaseValidator<PaymentAllocationDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentAllocationValidator"/> class.
        /// </summary>
        public PaymentAllocationValidator()
        {
            RuleFor(x => x.InvoicesID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_INVOICE_ID);

            RuleFor(x => x.PaymentID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID);

            RuleFor(x => x.AmountApplied)
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);

            RuleFor(x => x.TransactionType)
                .IsInEnum()
                .WithMessage("TransactionType is invalid.");
        }
    }
}
