using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PostsOnDutyValidator : AbstractValidator<PostsOnDutyDto>
    {
        public PostsOnDutyValidator()
        {
            RuleFor(x => x.AdminOnDutyID)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOBS_ON_DUTY_ADMIN_ON_DUTY_ID)
                .When(x => !string.IsNullOrEmpty(x.AdminOnDutyID));

            RuleFor(x => x.UnitName)
                .MaximumLength(35).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOBS_ON_DUTY_UNIT_NAME)
                .When(x => !string.IsNullOrEmpty(x.UnitName));

            RuleFor(x => x.PostName)
                .MaximumLength(45).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOBS_ON_DUTY_JOB_NAME)
                .When(x => !string.IsNullOrEmpty(x.PostName));

            RuleFor(x => x.StartDate)
                .MaximumLength(12).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOBS_ON_DUTY_START_DATE)
                .When(x => !string.IsNullOrEmpty(x.StartDate));

            RuleFor(x => x.ListOrder)
                .MaximumLength(250).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOBS_ON_DUTY_LIST_ORDER)
                .When(x => !string.IsNullOrEmpty(x.ListOrder));
        }
    }
}

