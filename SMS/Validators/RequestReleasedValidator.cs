using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequestScheduleValidator : AbstractValidator<RequestScheduleDto>
    {
        public RequestScheduleValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_EQUIPMENT_REQUEST_ID);

            RuleFor(x => x.GeneratedScheduleCount)
                .GreaterThanOrEqualTo(0)
                .When(x => x.GeneratedScheduleCount.HasValue);
        }
    }
}

