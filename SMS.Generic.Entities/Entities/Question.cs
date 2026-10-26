using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMS.Generic.Enums;

namespace SMS.Generic.Entities.Entities
{
    /// <summary>
    /// Represents a question entity in the system.
    /// </summary>
    [Table("Question", Schema = "dbo")]
    public class Question
    {
        /// <summary>
        /// Gets or sets the unique identifier for the question.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the activity form identifier (foreign key to ActivityForm).
        /// </summary>
        [Required]
        public Guid QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        [Column(TypeName = "nvarchar(max)")]
        public string? QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        [Required]
        [Column(TypeName = "int")]
        public QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets whether the question is required.
        /// </summary>
        [Required]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the display order of the question.
        /// </summary>
        [Required]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Navigation property for the activity form.
        /// </summary>
        [ForeignKey(nameof(QuestionnaireId))]
        public Questionnaire? Questionnaire { get; set; }
    }
}
