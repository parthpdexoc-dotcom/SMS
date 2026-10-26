using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PaymentUpdateAllocationAmountValidator : AbstractValidator<PaymentUpdateAllocationAmountDto>
    {
        public PaymentUpdateAllocationAmountValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID);

            RuleFor(x => x.Allocations)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);

            RuleForEach(x => x.Allocations).ChildRules(allocation =>
            {
                allocation.RuleFor(a => a.PaymentAllocationId)
                    .NotEqual(Guid.Empty)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);

                allocation.RuleFor(a => a.InvoiceId)
                    .NotEqual(Guid.Empty)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);

                allocation.RuleFor(a => a.Amount)
                    .GreaterThan(0)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);
            });
        }
    }
}
