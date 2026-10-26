using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentPolicyValidator : AbstractValidator<DepartmentPolicyDto>
    {
        public DepartmentPolicyValidator()
        {
            RuleFor(x => x.PostsTakenPerDay)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_JOBS_TAKEN_PER_DAY_INVALID);

            RuleFor(x => x.HoursPerDayWorked)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_HOURS_PER_DAY_INVALID);

            RuleFor(x => x.HoursPerWeekWorked)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_HOURS_PER_WEEK_INVALID);

            RuleFor(x => x.HoursPerMonthWorked)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_HOURS_PER_MONTH_INVALID);

            RuleFor(x => x.DeptOpenPostsTakenPerDay)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_DEPT_OPEN_JOBS_TAKEN_PER_DAY_INVALID);

            RuleFor(x => x.TenurePostsAllowedPerWeek)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_TENURE_JOBS_ALLOWED_PER_WEEK_INVALID);

            RuleFor(x => x.TenurePostsAllowedPerDay)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_TENURE_JOBS_ALLOWED_PER_DAY_INVALID);

            RuleFor(x => x.DeptOpenPostsTakenPerWeek)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_DEPT_OPEN_JOBS_TAKEN_PER_WEEK_INVALID);

            RuleFor(x => x.PreviewableHours)
                .GreaterThanOrEqualTo((short)0).When(x => x.PreviewableHours.HasValue)
                .WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_PREVIEWABLE_HOURS_INVALID);

            RuleFor(x => x.MinHoursPaid)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_MIN_HOURS_PAID_INVALID);

            RuleFor(x => x.OverlapJobConflictConfirmation)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_POLICY_OVERLAP_JOB_CONFLICT_CONFIRMATION);

            RuleFor(x => x.SupervisorsCanViewLowerEmpTypeJobs)
                .NotNull().WithMessage(ModelValidationErrorCodes.REQUIRED_DEPARTMENT_POLICY_SUPERVISORS_CAN_VIEW_LOWER_EMP_TYPE_JOBS);

            RuleFor(x => x.NumberOfEmployeesBeforeSupervisorRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ModelValidationErrorCodes.DEPARTMENT_POLICY_NUMBER_OF_EMPLOYEES_BEFORE_SUPERVISOR_INVALID);
        }
    }
}
