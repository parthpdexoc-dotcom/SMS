using System;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Answers operations.
    /// </summary>
    public class AnswersDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the answer.
        /// </summary>
        public Guid? AnswerId { get; set; }

        /// <summary>
        /// Gets or sets the response identifier.
        /// </summary>
        public Guid? ResponseId { get; set; }

        /// <summary>
        /// Gets or sets the question identifier.
        /// </summary>
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the answer value as a JSON string.
        /// </summary>
        public string? Answer { get; set; }
    }
}
