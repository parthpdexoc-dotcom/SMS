using System;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for Response operations.
    /// </summary>
    public class ResponseDto : BaseDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the response.
        /// </summary>
        public Guid? ResponseId { get; set; }

        /// <summary>
        /// Gets or sets the questionnaire identifier.
        /// </summary>
        public Guid? QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the email of the submitter.
        /// </summary>
        public string? SubmitterEmail { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the response was submitted.
        /// </summary>
        public DateTime? SubmittedOn { get; set; }

        /// <summary>
        /// Gets or sets the post position identifier.
        /// </summary>
        public Guid? PostPositionId { get; set; }
    }
}
