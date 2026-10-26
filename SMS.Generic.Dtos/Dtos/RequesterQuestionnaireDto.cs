namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for PostQuestionnaire operations.
    /// </summary>
    public class RequesterQuestionnaireDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the PostQuestionnaire.
        /// </summary>
        public Guid? RequesterQuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the Post ID (foreign key to Post table).
        /// </summary>
        public Guid? RequesterId { get; set; }

        /// <summary>
        /// Gets or sets the Questionnaire ID (foreign key to Questionnaire table).
        /// </summary>
        public Guid? QuestionnaireId { get; set; }
    }
}
