using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class BusinessTypeValidator : BaseValidator<BusinessTypeDto>
    {
        public BusinessTypeValidator()
        {
            RuleFor(x => x.BusinessTypeShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_BUSINESS_TYPE_SHORT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_BUSINESS_TYPE_SHORT);

            RuleFor(x => x.BusinessTypeLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_BUSINESS_TYPE_LONG)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_BUSINESS_TYPE_LONG);
        }
    }
}