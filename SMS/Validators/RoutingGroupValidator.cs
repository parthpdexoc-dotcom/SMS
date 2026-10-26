using FluentValidation;
using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class RoutingGroupValidator : AbstractValidator<RoutingGroupDto>
    {
        public RoutingGroupValidator()
        {
            RuleFor(x => x.ApprovalType)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_ROUTING_GROUP_APPROVAL_TYPE);

            RuleFor(x => x.EmployeeIDs)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_ROUTING_GROUP_EMP_IDS)
                .NotEmpty().WithMessage(ModelValidationErrorCodes.REQUIRED_ROUTING_GROUP_EMP_IDS);

            RuleFor(x => x.ApproverLong)
                .MaximumLength(200).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_LONG)
                .When(x => !string.IsNullOrEmpty(x.ApproverLong));

            RuleFor(x => x.ApproverShort)
                .MaximumLength(50).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_SHORT)
                .When(x => !string.IsNullOrEmpty(x.ApproverShort));

            RuleFor(x => x.ApproverEmail)
                .MaximumLength(150).WithMessage(ModelValidationErrorCodes.MAXIMUM_LENGTH_ROUTING_GROUP_APPROVER_EMAIL)
                .Must(EmailAddressHelper.IsValidEmailAddress)
                .WithMessage(ModelValidationErrorCodes.INVALID_ROUTING_GROUP_APPROVER_EMAIL_FORMAT)
                .When(x => !string.IsNullOrEmpty(x.ApproverEmail));
        }
    }
}
