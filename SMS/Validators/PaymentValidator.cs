using FluentValidation;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Validators
{
    public class PaymentValidator : AbstractValidator<PaymentDto>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotEqual(Guid.Empty).WithMessage("RequesterID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.ReferenceNumber)
                .MaximumLength(50).WithMessage("ReferenceNumber cannot exceed 50 characters.");

            RuleFor(x => x.PaymentTypeID)
                .NotEmpty().WithMessage("PaymentTypeID is required.");
        }
    }
}
