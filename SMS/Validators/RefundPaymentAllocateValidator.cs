using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RefundPaymentAllocateValidator : AbstractValidator<RefundPaymentAllocateDto>
    {
        public RefundPaymentAllocateValidator()
        {
            RuleFor(x => x.RefundPayment)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.INVALID_REFUND_PAYMENT_AMOUNT);

            RuleFor(x => x.RefundPayment.RequesterID)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_INVOICE_PAYMENT_REQUESTER_ID);

            RuleFor(x => x.RefundPayment.Amount)
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_REFUND_PAYMENT_AMOUNT);

            RuleFor(x => x.RefundPayment.PaymentTypeID)
                .NotEmpty()
                .WithMessage("PaymentTypeID is required.");

            RuleFor(x => x.Payments)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REFUND_PAYMENT_ALLOCATIONS);

            RuleForEach(x => x.Payments).ChildRules(payment =>
            {
                payment.RuleFor(p => p.PaymentId)
                    .NotEqual(Guid.Empty)
                    .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID);

                payment.RuleFor(p => p.Amount)
                    .GreaterThan(0)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);
            });
        }
    }
}
