using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PostPositionValidator : AbstractValidator<PostPositionDto>
    {
        public PostPositionValidator()
        {
            // 🔹 Required FK fields
            RuleFor(x => x.PostID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_ID);

            // 🔹 Date & Time (optional but must be valid if provided)
            RuleFor(x => x.DateOfWork)
                .NotNull()
                .When(x => x.PostStartTime.HasValue || x.PostEndTime.HasValue)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_DATE_OF_WORK);

            RuleFor(x => x.PostStartTime)
                .NotNull()
                .When(x => x.DateOfWork.HasValue)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_START_TIME);

            // 🔹 Length fields
            // LengthOfWork is now an int, validation ensures it's positive when StartTime is provided
            RuleFor(x => x.LengthOfWork)
                .GreaterThan(0)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_LENGTH_OF_WORK)
                .When(x => x.PostStartTime.HasValue);

            RuleFor(x => x.CallSign)
                .MaximumLength(50)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CALL_SIGN)
                .When(x => !string.IsNullOrEmpty(x.CallSign));

            // 🔹 Department-defined values
            RuleFor(x => x.PostDepartmentDefinedFieldValue1)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue1));

            RuleFor(x => x.PostDepartmentDefinedFieldValue2)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue2));

            RuleFor(x => x.PostDepartmentDefinedFieldValue3)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue3));

            RuleFor(x => x.PostDepartmentDefinedFieldValue4)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue4));

            RuleFor(x => x.PostDepartmentDefinedFieldValue5)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue5));

            RuleFor(x => x.PostDepartmentDefinedFieldValue6)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue6));

            RuleFor(x => x.PostDepartmentDefinedFieldValue7)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue7));

            RuleFor(x => x.PostDepartmentDefinedFieldValue8)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue8));

            RuleFor(x => x.PostDepartmentDefinedFieldValue9)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue9));

            RuleFor(x => x.PostDepartmentDefinedFieldValue10)
                .MaximumLength(200)
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_JOB_DEPARTMENT_DEFINED_VALUE)
                .When(x => !string.IsNullOrEmpty(x.PostDepartmentDefinedFieldValue10));
        }
    }
}

