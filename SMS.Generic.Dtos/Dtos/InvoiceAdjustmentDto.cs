using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for InvoiceAdjustment operations.
    /// </summary>
    public class InvoiceAdjustmentDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice adjustment.
        /// </summary>
        public long AdjustmentId { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the invoice ID.
        /// </summary>
        [MaxLength(50, ErrorMessage = "InvoiceID cannot exceed 50 characters")]
        public string? InvoiceID { get; set; }

        /// <summary>
        /// Gets or sets the previous value.
        /// </summary>
        public decimal? PreviousValue { get; set; }

        /// <summary>
        /// Gets or sets the updated value.
        /// </summary>
        public decimal? UpdatedValue { get; set; }

        /// <summary>
        /// Gets or sets the user ID who updated the record.
        /// </summary>
        public long? UpdatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the record was updated.
        /// </summary>
        public DateTime? UpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string? Comments { get; set; }

        /// <summary>
        /// Gets or sets the updated field name.
        /// </summary>
        [MaxLength(50, ErrorMessage = "UpdatedField cannot exceed 50 characters")]
        public string? UpdatedField { get; set; }
    }
}

