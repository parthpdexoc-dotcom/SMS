using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for PermitApprovalRoutingDto.
    /// </summary>
    public class PermitApprovalRoutingValidator : AbstractValidator<PermitApprovalRoutingDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PermitApprovalRoutingValidator"/> class.
        /// </summary>
        public PermitApprovalRoutingValidator()
        {
            RuleFor(x => x.RequestID)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_PERMIT_APPROVAL_ROUTING_REQUEST_ID);

            RuleFor(x => x.RoutingGroupId)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_PERMIT_APPROVAL_ROUTING_ROUTING_GROUP_ID);
        }
    }
}
