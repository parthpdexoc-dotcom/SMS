using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for ClosedPostPositionDto.
    /// </summary>
    public class ClosedPostPositionValidator : AbstractValidator<ClosedPostPositionDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionValidator"/> class.
        /// </summary>
        public ClosedPostPositionValidator()
        {
            // PostPositionID is required and cannot be an empty GUID
            RuleFor(x => x.PostPositionID)
                .NotNull()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_JOB_POSITION_ID)
                .Must(id => id.HasValue && id.Value != Guid.Empty)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_JOB_POSITION_ID);

            RuleFor(x => x.HoursWorked)
                .GreaterThanOrEqualTo(0)
                .When(x => x.HoursWorked.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_HOURS_WORKED);

            RuleFor(x => x.MinimumHoursDue)
                .GreaterThanOrEqualTo(0)
                .When(x => x.IsMinimumDue == true && x.MinimumHoursDue.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_MINIMUM_HOURS_DUE);

            RuleFor(x => x)
                .Must(dto => !dto.CloseDateTime.HasValue || !dto.WorkStartDate.HasValue ||
                             dto.CloseDateTime.Value.Date >= dto.WorkStartDate.Value.ToDateTime(TimeOnly.MinValue))
                .WithMessage(ModelValidationErrorCodes.CLOSED_JOB_POSITION_CLOSE_DATE_TIME_BEFORE_WORK_START);

            RuleFor(x => x.QuestionaireResponseID)
                .NotEmpty()
                .When(x => x.QuestionaireRequired == true)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_QUESTIONAIRE_RESPONSE_ID);

            RuleFor(x => x.Comment)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrEmpty(x.Comment))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CLOSED_JOB_POSITION_COMMENT);

            RuleFor(x => x.CloseBy)
                .MaximumLength(255)
                .When(x => !string.IsNullOrEmpty(x.CloseBy))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CLOSED_JOB_POSITION_CLOSE_BY);

        }
    }
}
