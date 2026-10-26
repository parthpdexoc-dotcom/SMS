using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a department questionnaire entity in the system.
    /// </summary>
    [Table("DepartmentQuestionnaire", Schema = "dbo")]
    public class DepartmentQuestionnaire : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the department questionnaire.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DepartmentQuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the department.
        /// </summary>
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the questionnaire.
        /// </summary>
        public Guid QuestionnaireId { get; set; }

        /// <summary>
        /// Navigation property to the department.
        /// </summary>
        [ForeignKey(nameof(DepartmentID))]
        public virtual Department? Department { get; set; }

        /// <summary>
        /// Navigation property to the questionnaire.
        /// </summary>
        [ForeignKey(nameof(QuestionnaireId))]
        public virtual Questionnaire? Questionnaire { get; set; }

        /// <summary>
        /// Navigation property to the admin who created the record.
        /// </summary>
        [ForeignKey(nameof(CreatedUserID))]
        public virtual Employee? CreatedByEmployee { get; set; }

        /// <summary>
        /// Navigation property to the employee who edited the record.
        /// </summary>
        [ForeignKey(nameof(EditedUserID))]
        public virtual Employee? EditedByEmployee { get; set; }
    }
}
