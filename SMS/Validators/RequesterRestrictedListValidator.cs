using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequesterRestrictedListValidator : BaseValidator<RequesterRestrictedListDto>
    {
        public RequesterRestrictedListValidator()
        {
            RuleFor(x => x.RequesterID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_RESTRICTED_LIST_REQUESTER_ID);

            RuleFor(x => x.EffectiveStartDate)
                .NotEqual(default(DateOnly)).WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_RESTRICTED_LIST_EFFECTIVE_START_DATE);

            RuleFor(x => x.EffectiveEndDate)
                .NotEqual(default(DateOnly)).WithMessage(ModelValidationErrorCodes.REQUIRED_REQUESTER_RESTRICTED_LIST_EFFECTIVE_END_DATE);
        }
    }
}

