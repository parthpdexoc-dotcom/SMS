using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Questionnaire operations.
    /// </summary>
    public class QuestionnaireDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the questionnaire.
        /// </summary>
        public Guid? QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the title of the questionnaire.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the questionnaire.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets whether the questionnaire is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the type of the questionnaire (Cancel = 0, Close = 1).
        /// </summary>
        public QuestionnaireType QuestionnaireType { get; set; }

        /// <summary>
        /// Gets or sets the source questionnaire identifier when this questionnaire is derived from another.
        /// </summary>
        public Guid? SourceQuestionnaireId { get; set; }
    }
}
