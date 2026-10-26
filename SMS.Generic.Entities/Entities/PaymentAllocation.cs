using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Links a payment to an invoice with the amount applied to that invoice.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(AmountApplied), OnlyAttributedProperties = true)]
    [Table("PaymentAllocations", Schema = "dbo")]
    public class PaymentAllocation : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentAllocationID { get; set; }

        [Required]
        [AuditedProperty("Invoice", LookupEntityType = typeof(Invoice), LookupDisplayPropertyName = nameof(Invoice.InvoiceNo))]
        public Guid InvoicesID { get; set; }

        [Required]
        [AuditedProperty("Payment", LookupEntityType = typeof(Payment), LookupDisplayPropertyName = nameof(Payment.PaymentNumber))]
        public Guid PaymentID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Amount applied")]
        public decimal AmountApplied { get; set; }

        [Column(TypeName = "datetime2")]
        [AuditedProperty("Allocation date")]
        public DateTime AllocationDate { get; set; }

        [AuditedProperty("Transaction type")]
        public PaymentAllocationTransactionType TransactionType { get; set; } = PaymentAllocationTransactionType.Debit;

        [MaxLength(500)]
        [AuditedProperty("Reason")]
        public string? Reason { get; set; }

        [ForeignKey(nameof(InvoicesID))]
        public virtual Invoice? Invoice { get; set; }

        [ForeignKey(nameof(PaymentID))]
        public virtual Payment? Payment { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }
    }
}
