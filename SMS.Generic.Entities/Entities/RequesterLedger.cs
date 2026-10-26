using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(Description), OnlyAttributedProperties = true)]
    [Table("RequesterLedgers", Schema = "dbo")]
    public class RequesterLedger : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequesterLedgerID { get; set; }

        [Required]
        [AuditedProperty("Requester", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public Guid RequesterID { get; set; }

        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        [Required]
        [MaxLength(1000)]
        [AuditedProperty("Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [AuditedProperty("Transaction date")]
        public DateTime TransactionDateTime { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [AuditedProperty("Opening balance")]
        public decimal OpeningBalance { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [AuditedProperty("Amount")]
        public decimal Amount { get; set; }

        [Required]
        [AuditedProperty("Transaction type")]
        public RequesterLedgerTransactionType TransactionType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [AuditedProperty("Balance")]
        public decimal Balance { get; set; }

        [Required]
        [AuditedProperty("Source type")]
        public RequesterLedgerSourceType SourceType { get; set; }

        [AuditedProperty("Source ID")]
        public Guid? SourceID { get; set; }

        [MaxLength(30)]
        [AuditedProperty("Source number")]
        public string? SourceNumber { get; set; }

        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }
    }
}
