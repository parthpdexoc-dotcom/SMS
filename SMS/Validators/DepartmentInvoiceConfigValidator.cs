using FluentValidation;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Models.Constants;

namespace SMS.Validators
{
    public class DepartmentInvoiceConfigValidator : BaseValidator<DepartmentInvoiceConfigDto>
    {
        public DepartmentInvoiceConfigValidator()
        {
            RuleFor(x => x.FiscalYear)
                .MaximumLength(5).WithMessage("FiscalYear cannot exceed 5 characters")
                .When(x => !string.IsNullOrEmpty(x.FiscalYear));

            RuleFor(x => x.StaffReimbursement)
                .GreaterThanOrEqualTo(0).WithMessage("StaffReimbursement cannot be negative")
                .When(x => x.StaffReimbursement.HasValue);

            RuleFor(x => x.VehicleReimbursement)
                .GreaterThanOrEqualTo(0).WithMessage("VehicleReimbursement cannot be negative")
                .When(x => x.VehicleReimbursement.HasValue);

            RuleFor(x => x.MileageReimbursedRate)
                .GreaterThanOrEqualTo(0).WithMessage("MileageReimbursedRate cannot be negative")
                .When(x => x.MileageReimbursedRate.HasValue);

            RuleFor(x => x.DefaultAdminFeeDollar)
                .GreaterThanOrEqualTo(0).WithMessage("DefaultAdminFeeDollar cannot be negative")
                .When(x => x.DefaultAdminFeeDollar.HasValue);

            RuleFor(x => x.DefaultAdminFeePercentage)
                .GreaterThanOrEqualTo(0).WithMessage("DefaultAdminFeePercentage cannot be negative")
                .When(x => x.DefaultAdminFeePercentage.HasValue);

            RuleFor(x => x.DefaultPayrollTax)
                .GreaterThanOrEqualTo(0).WithMessage("DefaultPayrollTax cannot be negative")
                .When(x => x.DefaultPayrollTax.HasValue);

            RuleFor(x => x.TotalCombinedAdminFees)
                .GreaterThanOrEqualTo(0).WithMessage("TotalCombinedAdminFees cannot be negative")
                .When(x => x.TotalCombinedAdminFees.HasValue);

            RuleFor(x => x.DepartmentAdminFee)
                .GreaterThanOrEqualTo(0).WithMessage("DepartmentAdminFee cannot be negative")
                .When(x => x.DepartmentAdminFee.HasValue);

            RuleFor(x => x.CancellationMinimumFee)
                .GreaterThanOrEqualTo(0).WithMessage("CancellationMinimumFee cannot be negative")
                .When(x => x.CancellationMinimumFee.HasValue);

            RuleFor(x => x.MinimumFee)
                .GreaterThanOrEqualTo(0).WithMessage("MinimumFee cannot be negative")
                .When(x => x.MinimumFee.HasValue);

            RuleFor(x => x.AgencySupportFee)
                .GreaterThanOrEqualTo(0).WithMessage("AgencySupportFee cannot be negative")
                .When(x => x.AgencySupportFee.HasValue);

            RuleFor(x => x.PayableDirectlyTo)
                .NotEmpty().WithMessage("PayableDirectlyTo is required")
                .MaximumLength(200).WithMessage("PayableDirectlyTo cannot exceed 200 characters");
        }
    }
}
