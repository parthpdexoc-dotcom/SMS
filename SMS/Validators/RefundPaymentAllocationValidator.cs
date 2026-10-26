using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RefundPaymentAllocationValidator : BaseValidator<RefundPaymentAllocationDto>
    {
        public RefundPaymentAllocationValidator()
        {
            RuleFor(x => x.PaymentID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID);

            RuleFor(x => x.RefundPaymentID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.INVALID_REFUND_PAYMENT_AMOUNT);

            RuleFor(x => x.AmountApplied)
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);

            RuleFor(x => x.TransactionType)
                .IsInEnum()
                .WithMessage("TransactionType is invalid.");
        }
    }
}
