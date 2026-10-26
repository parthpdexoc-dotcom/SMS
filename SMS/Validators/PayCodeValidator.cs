using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PayCodeValidator : BaseValidator<PayCodeDto>
    {
        public PayCodeValidator()
        {
            RuleFor(x => x.PayCodeValue)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_PAY_CODE_VALUE)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_PAY_CODE_VALUE);

            RuleFor(x => x.PayCodeDescription)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_PAY_CODE_DESCRIPTION)
                .MaximumLength(1000).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_PAY_CODE_DESCRIPTION);
        }
    }
}

