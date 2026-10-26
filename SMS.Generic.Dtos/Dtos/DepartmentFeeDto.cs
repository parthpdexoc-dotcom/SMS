using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for DepartmentFee operations.
    /// </summary>
    public class DepartmentFeeDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department fee.
        /// </summary>
        public Guid? FeeId { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the fee.
        /// </summary>
        public string? FeeShort { get; set; }

        /// <summary>
        /// Gets or sets the long name or description of the fee.
        /// </summary>
        public string? FeeLong { get; set; }

        /// <summary>
        /// Gets or sets the fee type.
        /// </summary>
        public FeeType FeeType { get; set; }

        /// <summary>
        /// Gets or sets the default fee conditions. Mandatory when IsAdditionalFees is true.
        /// </summary>
        public int? DefaultFeeConditions { get; set; }

        /// <summary>
        /// Gets or sets whether additional fees apply. Default is false.
        /// </summary>
        public bool IsAdditionalFees { get; set; }

        /// <summary>
        /// Gets or sets the default fee amount.
        /// </summary>
        public decimal DefaultFee { get; set; }

        /// <summary>
        /// Gets or sets the additional fee amount.
        /// </summary>
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets the reimbursement destination.
        /// </summary>
        public ReimburseTo ReimburseTo { get; set; }

        /// <summary>
        /// Gets or sets whether the department fee is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the date from which this fee is effective.
        /// </summary>
        public DateOnly? EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the date after which this fee is no longer effective.
        /// </summary>
        public DateOnly? EffectiveEndDate { get; set; }

        /// <summary>
        /// Gets or sets whether this fee is preselected on new requests.
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
