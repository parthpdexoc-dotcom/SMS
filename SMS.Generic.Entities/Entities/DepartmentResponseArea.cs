using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a response area entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(DepartmentResponseAreaShort), OnlyAttributedProperties = true)]
    [Table("DepartmentResponseArea", Schema = "dbo")]
    public class DepartmentResponseArea : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the response area.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentResponseAreaID { get; set; }

        /// <summary>
        /// Gets or sets the department identifier.
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the short name of the response area.
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        [AuditedProperty("Title")]
        public string? DepartmentResponseAreaShort { get; set; }

        /// <summary>
        /// Gets or sets the long description of the response area.
        /// </summary>
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        [AuditedProperty("Description")]
        public string? DepartmentResponseAreaLong { get; set; }

        /// <summary>
        /// Gets or sets the status of the response area (true or false).
        /// </summary>
        [AuditedProperty("Active")]
        public bool? DepartmentResponseAreaStatus { get; set; }


        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this response area.
        /// </summary>
        [ForeignKey("CreatedUserID")]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this response area.
        /// </summary>
        [ForeignKey("EditedUserID")]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
