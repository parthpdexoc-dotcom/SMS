using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department payment type entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(Name), OnlyAttributedProperties = true)]
    [Table("PaymentType", Schema = "dbo")]
    public class PaymentType : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PaymentTypeID { get; set; }

        /// <summary>
        /// Foreign Key to Department
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Payment type name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [AuditedProperty("Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Whether this payment type is the default for the department.
        /// </summary>
        [AuditedProperty("Default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this payment type.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this payment type.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who deleted this payment type.
        /// </summary>
        [ForeignKey(nameof(DeletedUserID))]
        public virtual Employee? DeletedByEmployee { get; set; }
    }
}
