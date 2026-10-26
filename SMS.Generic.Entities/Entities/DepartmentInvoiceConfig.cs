using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department invoice configuration entity in the system.
    /// </summary>
    [Table("DepartmentInvoiceConfig", Schema = "dbo")]
    public class DepartmentInvoiceConfig : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice configuration (Primary Key).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InvoiceConfigID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (Foreign Key).
        /// </summary>
        [Required]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the fiscal year.
        /// </summary>
        [MaxLength(5)]
        [Column(TypeName = "varchar(5)")]
        public string? FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets the staff reimbursement amount.
        /// </summary>
        [Column(TypeName = "smallmoney")]
        public decimal? StaffReimbursement { get; set; }

        /// <summary>
        /// Gets or sets whether mileage reimbursement is eligible.
        /// </summary>
        public bool? MileageReimburseEligible { get; set; }

        /// <summary>
        /// Gets or sets the vehicle reimbursement amount.
        /// </summary>
        [Column(TypeName = "smallmoney")]
        public decimal? VehicleReimbursement { get; set; }

        /// <summary>
        /// Gets or sets the mileage reimbursed rate.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MileageReimbursedRate { get; set; }

        /// <summary>
        /// Gets or sets the default admin fee in dollars.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DefaultAdminFeeDollar { get; set; }

        /// <summary>
        /// Gets or sets the default admin fee percentage.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DefaultAdminFeePercentage { get; set; }

        /// <summary>
        /// Gets or sets the default payroll tax.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DefaultPayrollTax { get; set; }

        /// <summary>
        /// Gets or sets the total combined admin fees.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalCombinedAdminFees { get; set; }

        /// <summary>
        /// Gets or sets the department admin fee.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DepartmentAdminFee { get; set; }

        /// <summary>
        /// Gets or sets the cancellation minimum fee.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CancellationMinimumFee { get; set; }

        /// <summary>
        /// Gets or sets the minimum fee.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumFee { get; set; }

        /// <summary>
        /// Gets or sets the agency support fee.
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AgencySupportFee { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        public DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets who payments are payable directly to.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PayableDirectlyTo { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether payments are payable to the officer.
        /// </summary>
        public bool IsPayableToOfficer { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this invoice config.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this invoice config.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
