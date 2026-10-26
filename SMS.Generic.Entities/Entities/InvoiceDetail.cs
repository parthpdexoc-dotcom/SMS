using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a single line item (job detail) on an invoice.
    /// One invoice has many invoice details.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(JobId), OnlyAttributedProperties = true)]
    [Table("InvoiceDetails", Schema = "dbo")]
    public class InvoiceDetail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the invoice detail.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InvoiceDetailID { get; set; }

        /// <summary>
        /// Gets or sets the parent invoice identifier.
        /// </summary>
        [AuditedProperty("Invoice", LookupEntityType = typeof(Invoice), LookupDisplayPropertyName = nameof(Invoice.InvoiceNo))]
        public Guid InvoicesID { get; set; }

        /// <summary>
        /// Gets or sets the closed post position this line item is sourced from, when applicable.
        /// </summary>
        [AuditedProperty("Closed post position", LookupEntityType = typeof(ClosedPostPosition), LookupDisplayPropertyName = nameof(ClosedPostPosition.ClosedPostPositionID))]
        public Guid? ClosePostPositionID { get; set; }

        /// <summary>
        /// Gets or sets the financial impact type for this line item (None, Debit, Credit).
        /// Stored on InvoiceDetails (int NOT NULL).
        /// </summary>
        [AuditedProperty("Financial impact type")]
        public FinancialImpactType FinancialImpactType { get; set; }

        /// <summary>
        /// Gets or sets vehicle ownership from job close (City, Employee, or Other).
        /// </summary>
        [AuditedProperty("Vehicle owned by")]
        public VehicleOwnedByType? VehicleOwnedBy { get; set; }

        /// <summary>
        /// Gets or sets who payments are payable directly to for this line item.
        /// </summary>
        [AuditedProperty("Payable directly to")]
        public string? PayableDirectlyTo { get; set; }

        /// <summary>
        /// Gets or sets whether payments are payable to the officer for this line item.
        /// </summary>
        [AuditedProperty("Payable to officer")]
        public bool IsPayableToOfficer { get; set; }

        /// <summary>
        /// Gets or sets the job identifier (e.g. "6165716").
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Job ID")]
        public string? JobId { get; set; }

        /// <summary>
        /// Gets or sets the job name.
        /// </summary>
        [MaxLength(500)]
        [AuditedProperty("Job name")]
        public string? JobName { get; set; }

        /// <summary>
        /// Gets or sets the date of the job.
        /// </summary>
        [Column(TypeName = "date")]
        [AuditedProperty("Job date")]
        public DateOnly? JobDate { get; set; }

        /// <summary>
        /// Gets or sets the job start time (from post position).
        /// </summary>
        [Column(TypeName = "time")]
        [AuditedProperty("Job time")]
        public TimeOnly? JobTime { get; set; }

        /// <summary>
        /// Gets or sets the post length in minutes for this line item.
        /// </summary>
        [AuditedProperty("Post length")]
        public int? PostLength { get; set; }

        /// <summary>
        /// Gets or sets the officer name.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Officer name")]
        public string? OfficerName { get; set; }

        /// <summary>
        /// Gets or sets the hourly or fixed rate.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Rate")]
        public decimal? Rate { get; set; }

        /// <summary>
        /// Gets or sets the alcohol special rate portion applied to this line when the invoice was generated.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Special rate")]
        public decimal? SpecialRate { get; set; }

        /// <summary>
        /// Gets or sets the number of hours worked.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Hours")]
        public decimal? Hours { get; set; }

        /// <summary>
        /// Gets or sets the total fees amount.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Fees amount")]
        public decimal? FeesAmount { get; set; }

        /// <summary>
        /// Gets or sets the vehicle fees amount for this line item (3 decimal places).
        /// </summary>
        [Column(TypeName = "decimal(18, 3)")]
        [AuditedProperty("Vehicle fee amount")]
        public decimal? VehicleFeeAmount { get; set; }

        /// <summary>
        /// Gets or sets the equipment fees amount for this line item.
        /// </summary>
        [Column(TypeName = "decimal(18, 3)")]
        [AuditedProperty("Equipment fee amount")]
        public decimal? EquipmentFeeAmount { get; set; }

        /// <summary>
        /// Gets or sets the total amount for this line item.
        /// </summary>
        [Column(TypeName = "decimal(18, 3)")]
        [AuditedProperty("Total amount")]
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// Navigation property to the parent invoice.
        /// </summary>
        [ForeignKey(nameof(InvoicesID))]
        public virtual Invoice? Invoice { get; set; }

        /// <summary>
        /// Navigation property to the closed post position, when linked.
        /// </summary>
        [ForeignKey(nameof(ClosePostPositionID))]
        public virtual ClosedPostPosition? ClosePostPosition { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }
    }
}
