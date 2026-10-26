using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a questionnaire entity in the system.
    /// </summary>
    [Table("Questionnaire", Schema = "dbo")]
    public class Questionnaire
    {
        /// <summary>
        /// Gets or sets the unique identifier for the questionnaire.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the title of the questionnaire.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the questionnaire.
        /// </summary>
        [MaxLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether the questionnaire is active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the type of the questionnaire (Cancel = 0, Close = 1).
        /// </summary>
        [Required]
        public QuestionnaireType QuestionnaireType { get; set; }

        /// <summary>
        /// Gets or sets the source questionnaire identifier when this questionnaire is derived from another.
        /// </summary>
        public Guid? SourceQuestionnaireId { get; set; }

        /// <summary>
        /// Navigation property to the source questionnaire when this questionnaire is derived from another.
        /// </summary>
        [ForeignKey(nameof(SourceQuestionnaireId))]
        public virtual Questionnaire? SourceQuestionnaire { get; set; }

        /// <summary>
        /// Navigation property to questionnaires derived from this one.
        /// </summary>
        [InverseProperty(nameof(SourceQuestionnaire))]
        public virtual ICollection<Questionnaire> DerivedQuestionnaires { get; set; } = new List<Questionnaire>();
    }
}
