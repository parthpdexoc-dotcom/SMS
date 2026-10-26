using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Common.Audit;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a skill entity in the system.
    /// </summary>
    [AuditedEntity(DisplayPropertyName = nameof(SkillShort), OnlyAttributedProperties = true)]
    [Table("Skill", Schema = "dbo")]
    public class Skill : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SkillID { get; set; }

        /// <summary>
        /// Foreign Key to Department
        /// </summary>
        [Required]
        [AuditedProperty("Department", LookupEntityType = typeof(Department), LookupDisplayPropertyName = nameof(Department.DepartmentName))]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Short name of the skill
        /// </summary>
        [MaxLength(50)]
        [AuditedProperty("Title")]
        public string? SkillShort { get; set; }

        /// <summary>
        /// Long description of the skill
        /// </summary>
        [MaxLength(200)]
        [AuditedProperty("Description")]
        public string? SkillLong { get; set; }

        /// <summary>
        /// Status of the skill (active/inactive)
        /// </summary>
        [AuditedProperty("Active")]
        public bool? SkillStatus { get; set; }


        /// <summary>
        /// Navigation property for the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property for the employee who created this skill.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property for the employee who last edited this skill.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
