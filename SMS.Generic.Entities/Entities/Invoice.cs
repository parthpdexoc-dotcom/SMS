using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    [AuditedEntity(DisplayPropertyName = nameof(InvoiceNo), OnlyAttributedProperties = true)]
    [Table("Invoices", Schema = "dbo")]
    public class Invoice : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InvoicesID { get; set; }

        [MaxLength(30)]
        [AuditedProperty("Invoice number")]
        public string? InvoiceNo { get; set; }

        [Column(TypeName = "smalldatetime")]
        [AuditedProperty("Date created")]
        public DateTime? DateCreated { get; set; }

        [Column(TypeName = "smalldatetime")]
        [AuditedProperty("Due date")]
        public DateTime? DueDate { get; set; }

        [Column(TypeName = "date")]
        [AuditedProperty("Start date")]
        public DateOnly StartDate { get; set; }

        [Column(TypeName = "date")]
        [AuditedProperty("End date")]
        public DateOnly EndDate { get; set; }

        [MaxLength(200)]
        [AuditedProperty("Billing from name")]
        public string? BillingFromName { get; set; }

        [MaxLength(500)]
        [AuditedProperty("Billing from address")]
        public string? BillingFromAddress { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Billing from phone")]
        public string? BillingFromPhoneNumber { get; set; }

        [MaxLength(100)]
        [AuditedProperty("Billing from email")]
        public string? BillingFromEmail { get; set; }

        [MaxLength(100)]
        [AuditedProperty("Billing from contact")]
        public string? BillingFromContact { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (foreign key to Department).
        /// Used to scope invoices per department in the shared GenericService.
        /// </summary>
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid? DepartmentID { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        [MaxLength(200)]
        [AuditedProperty("Billing to name")]
        public string? BillingToName { get; set; }

        [MaxLength(500)]
        [AuditedProperty("Billing to address")]
        public string? BillingToAddress { get; set; }

        [MaxLength(50)]
        [AuditedProperty("Billing to phone")]
        public string? BillingToPhoneNumber { get; set; }

        [MaxLength(100)]
        [AuditedProperty("Billing to email")]
        public string? BillingToEmail { get; set; }

        [MaxLength(100)]
        [AuditedProperty("Billing to contact")]
        public string? BillingToContact { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Officer hourly total")]
        public decimal? OfficerHourlyTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Officer fees total")]
        public decimal? OfficerFeesTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Agency fees total")]
        public decimal? AgencyFeesTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Vehicle fees total")]
        public decimal? VehicleFeesTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Equipment fees total")]
        public decimal? EquipmentFeesTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Total")]
        public decimal? Total { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Payments applied")]
        public decimal? PaymentsApplied { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [AuditedProperty("Balance due")]
        public decimal? BalanceDue { get; set; }

        /// <summary>
        /// Gets or sets whether the invoice has been fully paid.
        /// </summary>
        [AuditedProperty("Is paid")]
        public bool IsPaid { get; set; }

        /// <summary>
        /// Gets or sets the requester identifier (foreign key).
        /// </summary>
        [AuditedProperty("Requester", LookupEntityType = typeof(Requester), LookupDisplayPropertyName = nameof(Requester.Name))]
        public Guid? RequesterID { get; set; }

        /// <summary>
        /// Navigation property to the requester.
        /// </summary>
        [ForeignKey(nameof(RequesterID))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Gets or sets the related request identifier (foreign key).
        /// </summary>
        [AuditedProperty("Request", LookupEntityType = typeof(Request), LookupDisplayPropertyName = nameof(Request.Title))]
        public Guid? RequestId { get; set; }

        /// <summary>
        /// Gets or sets the post length in minutes (request/post context).
        /// </summary>
        [AuditedProperty("Post length")]
        public int? PostLength { get; set; }

        /// <summary>
        /// Navigation property to the related request.
        /// </summary>
        [ForeignKey(nameof(RequestId))]
        public virtual Request? Request { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }

        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

        /// <summary>
        /// Payment amounts applied to this invoice.
        /// </summary>
        public virtual ICollection<PaymentAllocation> PaymentAllocations { get; set; } = new List<PaymentAllocation>();
    }
}