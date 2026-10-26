using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;
using SMS.Helpers;

namespace SMS.Validators
{
    public class PostDocumentValidator : AbstractValidator<PostDocumentDto>
    {
        public PostDocumentValidator()
        {
            RuleFor(x => x.DocumentName)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_DOCUMENT_NAME)
                .MaximumLength(100)
                .WithMessage(
                    ValidationMessageHelper.GetMessage(
                        ModelValidationErrorCodes.MAXIMUM_LENGTH_DOCUMENT_NAME,
                        100));

            RuleFor(x => x.DocumentPath)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_DOCUMENT_PATH)
                .MaximumLength(250)
                .WithMessage(
                    ValidationMessageHelper.GetMessage(
                        ModelValidationErrorCodes.MAXIMUM_LENGTH_DOCUMENT_PATH,
                        250));
        }

    }
}
