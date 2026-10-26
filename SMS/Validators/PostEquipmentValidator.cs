using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class PostEquipmentValidator : AbstractValidator<PostEquipmentDto>
    {
        public PostEquipmentValidator()
        {
            RuleFor(x => x.PostID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_EQUIPMENT_JOB_ID);

            RuleFor(x => x.DepartmentEquipmentID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_JOB_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID);

            RuleFor(x => x.EquipmentQuantity)
                .GreaterThan(0).WithMessage(ModelValidationErrorCodes.INVALID_JOB_EQUIPMENT_QUANTITY);

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage(ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE);
        }
    }
}
