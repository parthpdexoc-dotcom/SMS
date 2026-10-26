using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for department questionnaire service operations.
    /// </summary>
    public interface IDepartmentQuestionnaireService : IGenericService<DepartmentQuestionnaireDto, DepartmentQuestionnaire>
    {
        /// <summary>
        /// Gets questionnaires that are not assigned to any department (for SuperUser usage), paged.
        /// </summary>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>Paged questionnaires not assigned in DepartmentQuestionnaire; <c>TotalRecords</c> is the full filtered count.</returns>
        Task<PagedList<QuestionnaireDto>> GetQuestionnairesByAdminAsync(PageParam? pagination = null);

        /// <summary>
        /// Searches questionnaires by admin with optional search term and pagination.
        /// </summary>
        /// <param name="searchTerm">The search term to filter questionnaires.</param>
        /// <param name="pagination">The pagination parameters.</param>
        /// <returns>A paginated list of questionnaires matching the search criteria.</returns>
        Task<PagedList<QuestionnaireDto>> SearchQuestionnairesByAdminAsync(string? searchTerm, PageParam? pagination = null);

        /// <summary>
        /// Searches questionnaires assigned to the current user's department with optional search term and pagination.
        /// </summary>
        /// <param name="searchTerm">The search term to filter questionnaires.</param>
        /// <param name="pagination">The pagination parameters.</param>
        /// <returns>A paginated list of questionnaires assigned to the department from the token.</returns>
        Task<PagedList<DepartmentQuestionnaireResponseDto>> SearchQuestionnairesByDepartmentAsync(string? searchTerm, PageParam? pagination = null);

        /// <summary>
        /// Gets questionnaires assigned to the current user's department (for Employee usage).
        /// Department ID is read from the token. Returns questionnaires in both Questionnaire and DepartmentQuestionnaire for that department.
        /// </summary>
        /// <param name="pagination">Optional pagination parameters.</param>
        /// <returns>Questionnaires assigned to the department from the token.</returns>
        Task<IEnumerable<DepartmentQuestionnaireResponseDto>> GetQuestionnairesByDepartmentAsync(PageParam? pagination = null);
    }
}
