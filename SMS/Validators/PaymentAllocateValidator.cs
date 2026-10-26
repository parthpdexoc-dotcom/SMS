using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PaymentAllocateValidator : AbstractValidator<PaymentAllocateDto>
    {
        public PaymentAllocateValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_PAYMENT_ID);

            RuleFor(x => x.RequesterId)
                .NotEqual(Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_INVOICE_PAYMENT_REQUESTER_ID);

            RuleFor(x => x.Invoices)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATE_INVOICES);

            RuleForEach(x => x.Invoices).ChildRules(invoice =>
            {
                invoice.RuleFor(i => i.InvoiceId)
                    .NotEqual(Guid.Empty)
                    .WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_ALLOCATION_INVOICE_ID);

                invoice.RuleFor(i => i.Amount)
                    .GreaterThan(0)
                    .When(i => !i.ApplyFullAmount)
                    .WithMessage(ModelValidationErrorCodes.INVALID_PAYMENT_ALLOCATION_AMOUNT_APPLIED);
            });
        }
    }
}
