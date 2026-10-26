using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PaymentTypeValidator : BaseValidator<PaymentTypeDto>
    {
        public PaymentTypeValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_PAYMENT_TYPE_NAME)
                .MaximumLength(100).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_PAYMENT_TYPE_NAME);
        }
    }
}
