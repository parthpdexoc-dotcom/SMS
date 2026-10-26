using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Links a payment to a refund payment with the amount applied.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(AmountApplied), OnlyAttributedProperties = true)]
    [Table("RefundPaymentAllocations", Schema = "dbo")]
    public class RefundPaymentAllocation : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RefundPaymentAllocationID { get; set; }

        [Required]
        [AuditedProperty("Payment", LookupEntityType = typeof(Payment), LookupDisplayPropertyName = nameof(Payment.PaymentNumber))]
        public Guid PaymentID { get; set; }

        [Required]
        [AuditedProperty("Refund payment", LookupEntityType = typeof(RefundPayment), LookupDisplayPropertyName = nameof(RefundPayment.RefundPaymentNumber))]
        public Guid RefundPaymentID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Amount applied")]
        public decimal AmountApplied { get; set; }

        [Column(TypeName = "datetime2")]
        [AuditedProperty("Allocation date")]
        public DateTime AllocationDate { get; set; }

        [AuditedProperty("Transaction type")]
        public RefundPaymentAllocationTransactionType TransactionType { get; set; } = RefundPaymentAllocationTransactionType.Debit;

        [MaxLength(500)]
        [AuditedProperty("Reason")]
        public string? Reason { get; set; }

        [ForeignKey(nameof(PaymentID))]
        public virtual Payment? Payment { get; set; }

        [ForeignKey(nameof(RefundPaymentID))]
        public virtual RefundPayment? RefundPayment { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }
    }
}
