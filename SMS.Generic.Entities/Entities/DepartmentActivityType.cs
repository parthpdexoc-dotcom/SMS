using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department activity type entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(ActivityTypeShort), OnlyAttributedProperties = true)]
    [Table("DepartmentActivityType", Schema = "dbo")]
    public class DepartmentActivityType : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID) - Unique identifier for the activity type.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentActivityTypeId { get; set; }

        /// <summary>
        /// Foreign Key to Department table.
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Short name or code for the activity type.
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [AuditedProperty("Title")]
        public string? ActivityTypeShort { get; set; }

        /// <summary>
        /// Long description for the activity type.
        /// </summary>
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        [AuditedProperty("Description")]
        public string? ActivityTypeLong { get; set; }

        /// <summary>
        /// Status of the activity type (active/inactive).
        /// </summary>
        [AuditedProperty("Active")]
        public bool? ActivityTypeStatus { get; set; }


        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this activity type.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this activity type.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
