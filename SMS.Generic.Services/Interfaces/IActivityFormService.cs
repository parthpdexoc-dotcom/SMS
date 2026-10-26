using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for activity form service operations, including generic questionnaire delete.
    /// </summary>
    public interface IActivityFormService : IGenericService<QuestionnaireDto, Questionnaire>
    {
        /// <summary>
        /// Gets the complete questionnaire form data including questions and options by questionnaire ID.
        /// </summary>
        /// <param name="questionnaireId">The questionnaire ID.</param>
        /// <returns>The questionnaire form data with questions and options.</returns>
        Task<QuestionnaireFormDto?> GetQuestionnaireFormByIdAsync(Guid questionnaireId);

        /// <summary>
        /// Inserts or updates a complete questionnaire form including questions and options.
        /// </summary>
        /// <param name="formDto">The questionnaire form data to insert or update.</param>
        /// <returns>The upserted questionnaire form data with questions and options.</returns>
        Task<QuestionnaireFormDto> UpsertQuestionnaireFormAsync(QuestionnaireFormDto formDto);

        /// <summary>
        /// Enriches questionnaire submissions with question text and resolves option IDs to option text.
        /// </summary>
        /// <param name="submissions">The submissions to enrich.</param>
        /// <param name="submissionInternalData">Internal data containing parsed option IDs keyed by questionnaire ID.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task EnrichQuestionnaireSubmissionsAsync(
            List<QuestionnaireSubmissionDto> submissions,
            Dictionary<Guid, List<SMS.Generic.Dtos.Dtos.AnswerItemInternal>> submissionInternalData);
    }
}
