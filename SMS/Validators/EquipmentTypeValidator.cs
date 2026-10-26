using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class EquipmentTypeValidator : BaseValidator<EquipmentTypeDto>
    {
        public EquipmentTypeValidator()
        {
            RuleFor(x => x.EquipementTypeShort)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EQUIPMENT_TYPE_SHORT)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EQUIPMENT_TYPE_SHORT)
                .When(x => x.EquipementTypeShort != null);

            RuleFor(x => x.EquipementTypeLong)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_EQUIPMENT_TYPE_LONG)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_EQUIPMENT_TYPE_LONG)
                .When(x => x.EquipementTypeLong != null);
        }
    }
}

