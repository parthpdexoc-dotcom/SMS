using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(PaymentNumber), OnlyAttributedProperties = true)]
    [Table("Payments", Schema = "dbo")]
    public class Payment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentID { get; set; }

        [Required]
        [AuditedProperty("Requester", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public Guid RequesterID { get; set; }

        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [AuditedProperty("Amount")]
        public decimal Amount { get; set; }

        [Required]
        [AuditedProperty("Payment date")]
        public DateTime PaymentDate { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Reference number")]
        public string? ReferenceNumber { get; set; }

        /// <summary>
        /// Department-scoped payment number with P- prefix (e.g. P-202600001). Assigned on create only.
        /// </summary>
        [Required]
        [MaxLength(20)]
        [AuditedProperty("Payment number")]
        public string PaymentNumber { get; set; } = string.Empty;

        /// <summary>
        /// Department-scoped payment type.
        /// </summary>
        [AuditedProperty("Payment type", LookupEntityType = typeof(PaymentType), LookupDisplayPropertyName = "Name")]
        public Guid? PaymentTypeID { get; set; }

        [MaxLength(500)]
        [AuditedProperty("Deletion reason")]
        public string? Reason { get; set; }

        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        [ForeignKey(nameof(PaymentTypeID))]
        public virtual PaymentType? PaymentTypeMaster { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }

        /// <summary>
        /// Invoice allocations for this payment.
        /// </summary>
        public virtual ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();

        /// <summary>
        /// Refund allocations for this payment.
        /// </summary>
        public virtual ICollection<RefundPaymentAllocation> RefundPaymentAllocations { get; set; } = new List<RefundPaymentAllocation>();
    }
}
