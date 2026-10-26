using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RequestEquipmentValidator : AbstractValidator<RequestEquipmentDto>
    {
        public RequestEquipmentValidator()
        {
            RuleFor(x => x.RequestID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_EQUIPMENT_REQUEST_ID);

            RuleFor(x => x.DepartmentEquipmentID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_REQUEST_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID);

            RuleFor(x => x.EquipmentQuantity)
                .GreaterThan(0).WithMessage(ModelValidationErrorCodes.INVALID_REQUEST_EQUIPMENT_QUANTITY);

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage(ModelValidationErrorCodes.END_DATE_CANNOT_BE_LESS_THAN_START_DATE);
        }
    }
}
