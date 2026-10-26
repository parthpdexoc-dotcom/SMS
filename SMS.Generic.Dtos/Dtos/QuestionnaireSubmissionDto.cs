using System;
using System.Collections.Generic;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// DTO for a single questionnaire submission (response + answers) when closing/splitting a post position.
    /// </summary>
    public class QuestionnaireSubmissionDto
    {
        /// <summary>
        /// Questionnaire identifier.
        /// </summary>
        public Guid? QuestionnaireId { get; set; }

        /// <summary>
        /// Display title for the questionnaire (populated when loading closed details).
        /// </summary>
        public string? QuestionnaireTitle { get; set; }

        /// <summary>
        /// Answers for this questionnaire (question id and answer value).
        /// </summary>
        public List<QuestionnaireAnswerItemDto>? Answers { get; set; }
    }

    /// <summary>
    /// A single answer item within a questionnaire submission.
    /// </summary>
    public class QuestionnaireAnswerItemDto
    {
        /// <summary>
        /// Question identifier.
        /// </summary>
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Question text (populated when loading closed details).
        /// </summary>
        public string? QuestionText { get; set; }

        /// <summary>
        /// Answer value (plain text answer, or comma-separated option text for multi-select questions).
        /// </summary>
        public string? Answer { get; set; }
    }

    /// <summary>
    /// Internal class to hold answer data during enrichment processing.
    /// Not exposed in API responses.
    /// </summary>
    public class AnswerItemInternal
    {
        /// <summary>
        /// Question identifier.
        /// </summary>
        public Guid? QuestionId { get; set; }

        /// <summary>
        /// Original answer value (may be JSON array of GUIDs for multi-select).
        /// </summary>
        public string? OriginalAnswer { get; set; }

        /// <summary>
        /// Parsed option IDs from JSON answer (for multi-select questions).
        /// </summary>
        public List<Guid>? SelectedOptionIds { get; set; }
    }
}
