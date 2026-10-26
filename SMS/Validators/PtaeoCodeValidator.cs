using FluentValidation;
using SMS.Generic.Dtos.Dtos;

namespace SMS.Validators
{
    /// <summary>
    /// Validator for PtaeoCodeDto.
    /// </summary>
    public class PtaeoCodeValidator : AbstractValidator<PtaeoCodeDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PtaeoCodeValidator"/> class.
        /// </summary>
        public PtaeoCodeValidator()
        {
            RuleFor(x => x.ProjectNumber)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.ProjectNumber));

            RuleFor(x => x.TaskNumber)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.TaskNumber));

            RuleFor(x => x.AwardNumber)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.AwardNumber));

            RuleFor(x => x.FundNumber)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.FundNumber));

            RuleFor(x => x.OrganizationName)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.OrganizationName));
        }
    }
}
