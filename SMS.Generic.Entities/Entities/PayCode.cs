using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a pay code entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(PayCodeValue), OnlyAttributedProperties = true)]
    [Table("PayCode", Schema = "dbo")]
    public class PayCode : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the pay code.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PayCodeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the pay code value.
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [AuditedProperty("Title")]
        public string? PayCodeValue { get; set; }

        /// <summary>
        /// Gets or sets the pay code description.
        /// </summary>
        [MaxLength(1000)]
        [Column(TypeName = "varchar(1000)")]
        [AuditedProperty("Description")]
        public string? PayCodeDescription { get; set; }

        /// <summary>
        /// Gets or sets the status of the pay code.
        /// </summary>
        [Required]
        [AuditedProperty("Active")]
        public bool PayCodeStatus { get; set; } = true;

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this pay code.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this pay code.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}

