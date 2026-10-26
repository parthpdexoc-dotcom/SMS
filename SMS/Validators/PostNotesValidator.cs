using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PostNotesValidator : AbstractValidator<PostNotesDto>
    {
        public PostNotesValidator()
        {
            RuleFor(x => x.PostID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_NOTES_JOB_ID);

            RuleFor(x => x.Notes)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_NOTES_NOTES)
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Comments)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_NOTES_COMMENTS)
                .When(x => !string.IsNullOrEmpty(x.Comments));
        }
    }
}

