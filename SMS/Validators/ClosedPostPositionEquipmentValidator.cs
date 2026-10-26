using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for ClosedPostPositionEquipmentDto.
    /// </summary>
    public class ClosedPostPositionEquipmentValidator : AbstractValidator<ClosedPostPositionEquipmentDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosedPostPositionEquipmentValidator"/> class.
        /// </summary>
        public ClosedPostPositionEquipmentValidator()
        {
            // ClosedPostPositionID required, exists, ownership and PostID match are validated in ClosedPostPositionEquipmentService (reference: ClosedPostPositionService).

            RuleFor(x => x.DepartmentEquipmentID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_DEPARTMENT_EQUIPMENT_ID);

            RuleFor(x => x.HoursWorked)
                .GreaterThanOrEqualTo(0)
                .When(x => x.HoursWorked.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_HOURS_WORKED);

            RuleFor(x => x.NoWorkReason)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.NoWorkReason))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CLOSED_JOB_POSITION_EQUIPMENT_NO_WORK_REASON);

            RuleFor(x => x.BreakTime)
                .GreaterThanOrEqualTo(0)
                .When(x => x.BreakTime.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_BREAK_TIME);

            RuleFor(x => x.UsedEquipmentQuantity)
                .GreaterThanOrEqualTo(0)
                .When(x => x.UsedEquipmentQuantity.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_USED_QUANTITY);

            RuleFor(x => x.EquipmentUsedHours)
                .GreaterThanOrEqualTo(0)
                .When(x => x.EquipmentUsedHours.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_EQUIPMENT_USED_HOURS);

            RuleFor(x => x.PaidHours)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PaidHours.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_PAID_HOURS);

            RuleFor(x => x.PayRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.PayRate.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_PAY_RATE);

            RuleFor(x => x.ReimbursePayRate)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ReimbursePayRate.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_REIMBURSE_PAY_RATE);

            RuleFor(x => x.VehNumber)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.VehNumber))
                .WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_CLOSED_JOB_POSITION_EQUIPMENT_VEH_NUMBER);

            RuleFor(x => x.VehicleOwnedBy)
                .IsInEnum()
                .When(x => x.VehicleOwnedBy.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_VEHICLE_OWNED_BY);

            RuleFor(x => x.VehicleOwnedBy)
                .NotEmpty()
                .When(x => x.EquipmentUsedIsVehicle == true)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_VEHICLE_OWNED_BY);

            RuleFor(x => x.StartMileage)
                .NotEmpty()
                .When(x => x.EquipmentUsedIsVehicle == true)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_START_MILEAGE);

            RuleFor(x => x.StartMileage)
                .GreaterThanOrEqualTo(0)
                .When(x => x.EquipmentUsedIsVehicle == true && x.StartMileage.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_START_MILEAGE);

            RuleFor(x => x.EndMileage)
                .NotEmpty()
                .When(x => x.EquipmentUsedIsVehicle == true)
                .WithMessage(ModelValidationErrorCodes.REQUIRED_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE);

            RuleFor(x => x.EndMileage)
                .GreaterThanOrEqualTo(0)
                .When(x => x.EquipmentUsedIsVehicle == true && x.EndMileage.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE_VALUE);

            RuleFor(x => x.EndMileage)
                .GreaterThanOrEqualTo(x => x.StartMileage)
                .When(x => x.EquipmentUsedIsVehicle == true && x.StartMileage.HasValue && x.EndMileage.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_CLOSED_JOB_POSITION_EQUIPMENT_END_MILEAGE);
        }
    }
}
