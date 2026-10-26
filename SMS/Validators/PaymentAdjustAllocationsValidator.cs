using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PaymentAdjustAllocationsValidator : AbstractValidator<PaymentAdjustAllocationsDto>
    {
        public PaymentAdjustAllocationsValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID);

            RuleFor(x => x.PaymentAmount)
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_INVOICE_PAYMENT_AMOUNT);

            RuleFor(x => x.PaymentDate)
                .NotEqual(default(DateTime))
                .WithMessage(ModelValidationErrorCodes.INVALID_INVOICE_PAYMENT_AMOUNT);

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(50)
                .WithMessage("ReferenceNumber cannot exceed 50 characters.");

            RuleFor(x => x.PaymentTypeID)
                .NotEmpty()
                .WithMessage("PaymentTypeID is required.");

            When(x => x.Allocations != null && x.Allocations.Count > 0, () =>
            {
                RuleForEach(x => x.Allocations).ChildRules(allocation =>
                {
                    allocation.RuleFor(a => a.PaymentAllocationId)
                        .NotEqual(Guid.Empty)
                        .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);

                    allocation.RuleFor(a => a.Amount)
                        .GreaterThan(0)
                        .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);
                });
            });
        }
    }
}
