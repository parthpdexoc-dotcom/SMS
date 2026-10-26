namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Option operations.
    /// </summary>
    public class OptionDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Option.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("optionId")]
        [System.Text.Json.Serialization.JsonPropertyName("optionId")]
        public Guid? OptionId { get; set; }

        /// <summary>
        /// Gets or sets the Question ID (foreign key to Question table).
        /// </summary>
        [Newtonsoft.Json.JsonProperty("questionId")]
        [System.Text.Json.Serialization.JsonPropertyName("questionId")]
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the option text.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("optionText")]
        [System.Text.Json.Serialization.JsonPropertyName("optionText")]
        public string? OptionText { get; set; }

        /// <summary>
        /// Gets or sets the display order of the option.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("displayOrder")]
        [System.Text.Json.Serialization.JsonPropertyName("displayOrder")]
        public int DisplayOrder { get; set; }
    }
}
