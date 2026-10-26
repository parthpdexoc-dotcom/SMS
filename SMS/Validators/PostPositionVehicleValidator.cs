using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for PostPositionVehicleDto.
    /// </summary>
    public class PostPositionVehicleValidator : AbstractValidator<PostPositionVehicleDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostPositionVehicleValidator"/> class.
        /// </summary>
        public PostPositionVehicleValidator()
        {
            RuleFor(x => x.PostPositionID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_POST_POSITION_VEHICLE_POST_POSITION_ID);

            RuleFor(x => x.DepartmentEquipmentID)
                .NotEmpty()
                .WithMessage(ModelValidationErrorCodes.REQUIRED_POST_POSITION_VEHICLE_DEPARTMENT_EQUIPMENT_ID);

            RuleFor(x => x.Charge)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Charge.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_POST_POSITION_VEHICLE_CHARGE);

            RuleFor(x => x.MileageCharge)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MileageCharge.HasValue)
                .WithMessage(ModelValidationErrorCodes.INVALID_POST_POSITION_VEHICLE_MILEAGE_CHARGE);
        }
    }
}
