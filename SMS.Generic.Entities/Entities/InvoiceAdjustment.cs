using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents an invoice adjustment entity in the system.
    /// </summary>
    [Table("InvoiceAdjustment", Schema = "dbo")]
    public class InvoiceAdjustment
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice adjustment.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AdjustmentId { get; set; }

        /// <summary>
        /// Gets or sets the department ID.
        /// </summary>
        public int? DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the invoice ID.
        /// </summary>
        [MaxLength(50)]
        public string? InvoiceID { get; set; }

        /// <summary>
        /// Gets or sets the previous value.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PreviousValue { get; set; }

        /// <summary>
        /// Gets or sets the updated value.
        /// </summary>
        [Column(TypeName = "decimal(10, 2)")]
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
        [MaxLength(50)]
        public string? UpdatedField { get; set; }
    }
}

