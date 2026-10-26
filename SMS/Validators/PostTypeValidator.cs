using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PostTypeValidator : AbstractValidator<PostTypeDto>
    {
        public PostTypeValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(35).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_TYPE_TITLE)
                .When(x => !string.IsNullOrEmpty(x.Title));

            RuleFor(x => x.Description)
                .MaximumLength(255).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_TYPE_DESCRIPTION)
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}

