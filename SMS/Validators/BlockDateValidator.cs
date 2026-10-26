using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for BlockDateDto using FluentValidation.
    /// </summary>
    public class BlockDateValidator : AbstractValidator<BlockDateDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlockDateValidator"/> class.
        /// </summary>
        public BlockDateValidator()
        {
            // Validate StartDateTime is required and not default
            RuleFor(x => x.StartDateTime)
                .NotEqual(default(DateTime))
                .WithMessage(ModelValidationErrorCodes.REQUIRED_BLOCK_DATE_START_DATE_TIME);

            // Validate EndDateTime is required and not default
            RuleFor(x => x.EndDateTime)
                .NotEqual(default(DateTime))
                .WithMessage(ModelValidationErrorCodes.REQUIRED_BLOCK_DATE_END_DATE_TIME);

            // Validate EndDateTime is greater than StartDateTime
            RuleFor(x => x)
                .Custom((model, context) =>
                {
                    if (model.StartDateTime != default(DateTime) && 
                        model.EndDateTime != default(DateTime) &&
                        model.EndDateTime <= model.StartDateTime)
                    {
                        context.AddFailure(
                            nameof(model.EndDateTime),
                            ModelValidationErrorCodes.END_DATE_TIME_CANNOT_BE_LESS_THAN_OR_EQUAL_START_DATE_TIME
                        );
                    }
                });

            // Validate PostID is not empty Guid when provided
            RuleFor(x => x.PostID)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.INVALID_BLOCK_DATE_JOB_ID)
                .When(x => x.PostID.HasValue);

            // Validate RequestID is not empty Guid when provided
            RuleFor(x => x.RequestID)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.INVALID_BLOCK_DATE_REQUEST_ID)
                .When(x => x.RequestID.HasValue);
        }
    }
}
