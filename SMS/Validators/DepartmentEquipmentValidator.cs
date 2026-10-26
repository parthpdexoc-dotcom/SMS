using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentEquipmentValidator : BaseValidator<DepartmentEquipmentDto>
    {
        public DepartmentEquipmentValidator()
        {
            //RuleFor(x => x.EquipmentTypeID)
            //    .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_EQUIPMENT_TYPE_ID);

            RuleFor(x => x.ChargedType)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_CHARGED_TYPE);

            RuleFor(x => x.Charge)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_CHARGE);

            RuleFor(x => x.ChargeMileage)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_CHARGE_MILEAGE);

            RuleFor(x => x.MileageCharge)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_MILEAGE_CHARGE);

            RuleFor(x => x.MileageCondition)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_MILEAGE_CONDITION);

            RuleFor(x => x.EquipmentStatus)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_EQUIPMENT_STATUS);

            RuleFor(x => x.RequiresApproval)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_REQUIRES_APPROVAL);

            RuleFor(x => x.RequiresUtilizationRecorded)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_EQUIPMENT_REQUIRES_UTILIZATION_RECORDED);
        }
    }
}

