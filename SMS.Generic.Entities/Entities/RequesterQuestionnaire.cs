using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a RequesterQuestionnaire entity in the system.
    /// </summary>
    [Table("RequesterQuestionnaire", Schema = "dbo")]
    public class RequesterQuestionnaire
    {
        /// <summary>
        /// Gets or sets the unique identifier for the RequesterQuestionnaire.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid RequesterQuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the Requester ID (foreign key to Requester table).
        /// </summary>
        [Required]
        public Guid RequesterId { get; set; }

        /// <summary>
        /// Gets or sets the Questionnaire ID (foreign key to Questionnaire table).
        /// </summary>
        [Required]
        public Guid QuestionnaireId { get; set; }

        /// <summary>
        /// Navigation property to the Requester.
        /// </summary>
        [ForeignKey(nameof(RequesterId))]
        public virtual Requester? Requester { get; set; }

        /// <summary>
        /// Navigation property to the Questionnaire.
        /// </summary>
        [ForeignKey(nameof(QuestionnaireId))]
        public virtual Questionnaire? Questionnaire { get; set; }
    }
}
