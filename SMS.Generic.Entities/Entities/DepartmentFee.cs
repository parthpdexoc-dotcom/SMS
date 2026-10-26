using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department fee entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(FeeShort), OnlyAttributedProperties = true)]
    [Table("DepartmentFee", Schema = "dbo")]
    public class DepartmentFee : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department fee (Primary Key).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FeeId { get; set; }

        /// <summary>
        /// Gets or sets the department identifier (Foreign Key to Department).
        /// </summary>
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the fee.
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Title")]
        public string FeeShort { get; set; } = null!;

        /// <summary>
        /// Gets or sets the long name or description of the fee.
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Description")]
        public string? FeeLong { get; set; }

        /// <summary>
        /// Gets or sets the fee type.
        /// </summary>
        [AuditedProperty("Fee type")]
        public int FeeType { get; set; }

        /// <summary>
        /// Gets or sets the default fee conditions. Mandatory when IsAdditionalFees is true.
        /// </summary>
        [AuditedProperty("Default fee conditions")]
        public int? DefaultFeeConditions { get; set; }

        /// <summary>
        /// Gets or sets whether additional fees apply. Default is false.
        /// </summary>
        [AuditedProperty("Additional fees")]
        public bool IsAdditionalFees { get; set; }

        /// <summary>
        /// Gets or sets the default fee amount.
        /// </summary>
        [Column(TypeName = "smallmoney")]
        [AuditedProperty("Default fee")]
        public decimal DefaultFee { get; set; }

        /// <summary>
        /// Gets or sets the additional fee amount.
        /// </summary>
        [Column(TypeName = "smallmoney")]
        [AuditedProperty("Additional fee")]
        public decimal AdditionalFee { get; set; }

        /// <summary>
        /// Gets or sets the reimbursement destination.
        /// </summary>
        [AuditedProperty("Reimburse to")]
        public int ReimburseTo { get; set; }

        /// <summary>
        /// Gets or sets whether the department fee is active.
        /// </summary>
        [Required]
        [AuditedProperty("Active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date from which this fee is effective.
        /// </summary>
        [Column(TypeName = "date")]
        [AuditedProperty("Effective start date")]
        public DateOnly? EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the date after which this fee is no longer effective.
        /// </summary>
        [Column(TypeName = "date")]
        [AuditedProperty("Effective end date")]
        public DateOnly? EffectiveEndDate { get; set; }

        /// <summary>
        /// Gets or sets whether this fee is preselected on new requests.
        /// </summary>
        [AuditedProperty("Default selection")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this department fee.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who edited this department fee.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
