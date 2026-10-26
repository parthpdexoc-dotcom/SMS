using SMS.Generic.Dtos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentInvoiceConfig operations.
    /// </summary>
    public class DepartmentInvoiceConfigDto : BaseDto ,IDepartmentScoped
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice configuration.
        /// </summary>
        public Guid InvoiceConfigID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (required for upsert; supplied in request body).
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the fiscal year.
        /// </summary>
        [MaxLength(5, ErrorMessage = "FiscalYear cannot exceed 5 characters")]
        public string? FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets the staff reimbursement amount.
        /// </summary>
        public decimal? StaffReimbursement { get; set; }

        /// <summary>
        /// Gets or sets whether mileage reimbursement is eligible.
        /// </summary>
        public bool? MileageReimburseEligible { get; set; }

        /// <summary>
        /// Gets or sets the vehicle reimbursement amount.
        /// </summary>
        public decimal? VehicleReimbursement { get; set; }

        /// <summary>
        /// Gets or sets the mileage reimbursed rate.
        /// </summary>
        public decimal? MileageReimbursedRate { get; set; }

        /// <summary>
        /// Gets or sets the default admin fee in dollars.
        /// </summary>
        public decimal? DefaultAdminFeeDollar { get; set; }

        /// <summary>
        /// Gets or sets the default admin fee percentage.
        /// </summary>
        public decimal? DefaultAdminFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets the default payroll tax.
        /// </summary>
        public decimal? DefaultPayrollTax { get; set; }

        /// <summary>
        /// Gets or sets the total combined admin fees.
        /// </summary>
        public decimal? TotalCombinedAdminFees { get; set; }

        /// <summary>
        /// Gets or sets the department admin fee.
        /// </summary>
        public decimal? DepartmentAdminFee { get; set; }

        /// <summary>
        /// Gets or sets the cancellation minimum fee.
        /// </summary>
        public decimal? CancellationMinimumFee { get; set; }

        /// <summary>
        /// Gets or sets the minimum fee.
        /// </summary>
        public decimal? MinimumFee { get; set; }

        /// <summary>
        /// Gets or sets the agency support fee.
        /// </summary>
        public decimal? AgencySupportFee { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets who payments are payable directly to.
        /// </summary>
        [MaxLength(200, ErrorMessage = "PayableDirectlyTo cannot exceed 200 characters")]
        public string PayableDirectlyTo { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether payments are payable to the officer.
        /// </summary>
        public bool IsPayableToOfficer { get; set; }
    }
}
