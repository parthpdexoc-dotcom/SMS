using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for DepartmentFeeDto using FluentValidation.
    /// </summary>
    public class DepartmentFeeDtoValidator : AbstractValidator<DepartmentFeeDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DepartmentFeeDtoValidator"/> class.
        /// </summary>
        public DepartmentFeeDtoValidator()
        {
            RuleFor(x => x.DepartmentID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_FEE_DEPARTMENT_ID);

            RuleFor(x => x.FeeShort)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_FEE_FEE_SHORT)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_FEE_FEE_SHORT);

            RuleFor(x => x.FeeLong)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_DEPARTMENT_FEE_FEE_LONG)
                .When(x => !string.IsNullOrEmpty(x.FeeLong));

            //RuleFor(x => x.FeeType)
            //    .IsInEnum()
            //    .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_FEE_TYPE)
            //    .NotEqual(FeeType.OneTime)
            //    .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_FEE_TYPE_ONE_TIME);
            RuleFor(x => x.FeeType)
                .IsInEnum()
                .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_FEE_TYPE);

            RuleFor(x => x.DefaultFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_DEFAULT_FEE);

            RuleFor(x => x.AdditionalFee)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_ADDITIONAL_FEE);

            RuleFor(x => x.ReimburseTo)
                .IsInEnum()
                .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_REIMBURSE_TO);

            //RuleFor(x => x.EffectiveStartDate)
            //    .NotNull()
            //    .WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_FEE_EFFECTIVE_START_DATE);

            //RuleFor(x => x.EffectiveEndDate)
            //    .GreaterThanOrEqualTo(x => x.EffectiveStartDate!.Value)
            //    .When(x => x.EffectiveStartDate.HasValue && x.EffectiveEndDate.HasValue)
            //    .WithMessage(ModelValidationErrorCodes.INVALID_DEPARTMENT_FEE_EFFECTIVE_DATE_RANGE);
        }
    }
}
