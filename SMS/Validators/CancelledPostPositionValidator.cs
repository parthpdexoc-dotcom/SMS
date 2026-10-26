using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for CancelledPostPositionDto.
    /// </summary>
    public class CancelledPostPositionValidator : AbstractValidator<CancelledPostPositionDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelledPostPositionValidator"/> class.
        /// </summary>
        public CancelledPostPositionValidator()
        {
            // EmployeeID is set from token via IEmployeeScoped - not validated from request

            RuleFor(x => x.PostID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CANCELLED_JOB_POSITION_JOB_ID);

            RuleFor(x => x.PostPositionID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CANCELLED_JOB_POSITION_JOB_POSITION_ID);

            RuleFor(x => x.QuestionaireResponseID)
                .NotEmpty()
                .When(x => x.QuestionaireRequired == true)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CANCELLED_JOB_POSITION_QUESTIONAIRE_RESPONSE_ID);

            RuleFor(x => x.WorkedHours)
                .GreaterThanOrEqualTo(0)
                .When(x => x.WorkedHours.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_WORKED_HOURS);

            RuleFor(x => x.RequiredHours)
                .GreaterThanOrEqualTo(0)
                .When(x => x.RequiredHours.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_REQUIRED_HOURS);

            RuleFor(x => x.Mileage)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Mileage.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_MILEAGE);

            RuleFor(x => x.PayRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PayRate.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_PAY_RATE);

            RuleFor(x => x.ReimbursePayRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ReimbursePayRate.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_REIMBURSE_PAY_RATE);

            RuleFor(x => x.CancelledDateTime)
                .Must(dt => !dt.HasValue || dt.Value <= DateTime.UtcNow)
                .WithMessage(ModelValidationErrorCodes.INVALID_CANCELLED_JOB_POSITION_CANCELLED_DATE_TIME_FUTURE);

            RuleFor(x => x.CancelledBy)
                .MaximumLength(255)
                .When(x => !string.IsNullOrEmpty(x.CancelledBy))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_CANCELLED_BY);

            RuleFor(x => x.IPAddressCancelledBy)
                .MaximumLength(255)
                .When(x => !string.IsNullOrEmpty(x.IPAddressCancelledBy))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_IP_ADDRESS_CANCELLED_BY);

            RuleFor(x => x.ReimburseStatus)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.ReimburseStatus))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CANCELLED_JOB_POSITION_REIMBURSE_STATUS);
        }
    }
}
