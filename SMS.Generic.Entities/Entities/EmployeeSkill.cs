using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a skill assignment for an employee with an effective date range.
    /// </summary>
    [Table("EmployeeSkill", Schema = "dbo")]
    public class EmployeeSkill : BaseEntity
    {
        /// <summary>
        /// Primary Key (GUID)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EmployeeSkillID { get; set; }

        /// <summary>
        /// Foreign key to Employee.
        /// </summary>
        [Required]
        public Guid EmployeeID { get; set; }

        /// <summary>
        /// Foreign key to Skill.
        /// </summary>
        [Required]
        public Guid SkillID { get; set; }

        /// <summary>
        /// Start date for which this skill is effective.
        /// </summary>
        [Column(TypeName = "date")]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Optional end date for which this skill is effective.
        /// </summary>
        [Column(TypeName = "date")]
        public DateOnly? EndDate { get; set; }

        [ForeignKey(nameof(EmployeeID))]
        public virtual Employee? Employee { get; set; }

        [ForeignKey(nameof(SkillID))]
        public virtual Skill? Skill { get; set; }

        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}

