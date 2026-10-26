using SMS.Generic.Enums;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Question operations.
    /// </summary>
    public class QuestionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the question.
        /// </summary>
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the activity form identifier (foreign key to ActivityForm).
        /// </summary>
        public Guid? QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the question text.
        /// </summary>
        public string? QuestionText { get; set; }

        /// <summary>
        /// Gets or sets the question type.
        /// </summary>
        public QuestionType QuestionType { get; set; }

        /// <summary>
        /// Gets or sets whether the question is required.
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the display order of the question.
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
