namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for complete questionnaire form data including questions and options.
    /// </summary>
    public class QuestionnaireFormDto
    {
        /// <summary>
        /// Gets or sets the questionnaire information.
        /// </summary>
        public QuestionnaireDto Questionnaire { get; set; } = null!;

        /// <summary>
        /// Gets or sets the list of questions with their options.
        /// </summary>
        public List<QuestionWithOptionsDto> Questions { get; set; } = new List<QuestionWithOptionsDto>();
    }

    /// <summary>
    /// Data Transfer Object for a question with its options.
    /// </summary>
    public class QuestionWithOptionsDto : QuestionDto
    {
        /// <summary>
        /// Gets or sets the list of options for this question.
        /// </summary>
        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }
}
