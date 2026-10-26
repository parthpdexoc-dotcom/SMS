namespace SMS.Generic.Enums
{
    /// <summary>
    /// Represents the type of question in an activity form.
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// Short answer question type.
        /// </summary>
        ShortAnswer = 0,

        /// <summary>
        /// Paragraph question type.
        /// </summary>
        Paragraph = 1,

        /// <summary>
        /// Radio (single choice) question type.
        /// </summary>
        Radio = 2,

        /// <summary>
        /// Multi choice (checkboxes) question type.
        /// </summary>
        MultiChoice = 3
    }
}
