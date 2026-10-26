using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Department fee option returned by the request-selection endpoint.
    /// Used on both Create Request (no requestId) and Edit Request (requestId provided).
    /// </summary>
    public class ApplicableDepartmentFeeDto
    {
        public Guid DepartmentFeeId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public FeeType FeeType { get; set; }

        public decimal DefaultFee { get; set; }

        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets the reimbursement destination.
        /// </summary>
        public ReimburseTo ReimburseTo { get; set; }

        /// <summary>
        /// True when this fee is marked as a default fee in the department configuration.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Create mode: true when IsDefault = true.
        /// Edit mode: true when the fee is saved in RequestDepartmentFee OR is a default fee.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Always false in create mode.
        /// In edit mode: true when the fee is already saved in the RequestDepartmentFee table for this request.
        /// Helps the UI distinguish pre-saved fees from newly-added default fees.
        /// </summary>
        public bool IsExisting { get; set; }
    }
}
