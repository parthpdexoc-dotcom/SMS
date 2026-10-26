using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequestNotesValidator : AbstractValidator<RequestNotesDto>
    {
        public RequestNotesValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_NOTES_REQUEST_ID);

            RuleFor(x => x.Notes)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_NOTES)
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.Comments)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_REQUEST_COMMENTS)
                .When(x => !string.IsNullOrEmpty(x.Comments));
        }
    }
}

