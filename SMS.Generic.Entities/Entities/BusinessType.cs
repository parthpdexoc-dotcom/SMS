using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a business type entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(BusinessTypeShort), OnlyAttributedProperties = true)]
    [Table("BusinessType", Schema = "dbo")]
    public class BusinessType : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the business type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid BusinessTypeID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the business type.
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [AuditedProperty("Title")]
        public string? BusinessTypeShort { get; set; }

        /// <summary>
        /// Gets or sets the long description of the business type.
        /// </summary>
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        [AuditedProperty("Description")]
        public string? BusinessTypeLong { get; set; }

        /// <summary>
        /// Gets or sets the status of the business type.
        /// </summary>
        [AuditedProperty("Active")]
        public bool? BusinessTypeStatus { get; set; }


        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this business type.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this business type.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}

