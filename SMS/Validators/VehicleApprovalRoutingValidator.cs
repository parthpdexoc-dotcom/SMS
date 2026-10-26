using FluentValidation;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for VehicleApprovalRoutingDto.
    /// </summary>
    public class VehicleApprovalRoutingValidator : AbstractValidator<VehicleApprovalRoutingDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VehicleApprovalRoutingValidator"/> class.
        /// </summary>
        public VehicleApprovalRoutingValidator()
        {
            RuleFor(x => x.ApproverID)
                .NotEmpty().WithMessage("Approver ID is required.");

            RuleFor(x => x.DepartmentEquipmentID)
                .NotEmpty().WithMessage("Department Equipment ID is required.");

            // VehicleApproverParentRouteId is optional (null for first record)
            // VehicleApproverIsParent will be set automatically by the service
            // VehicleApproverHasFinalAuthority is a boolean, no validation needed
        }
    }
}

