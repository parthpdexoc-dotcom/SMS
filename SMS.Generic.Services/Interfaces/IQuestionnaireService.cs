using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for questionnaire service operations.
    /// </summary>
    public interface IQuestionnaireService : IGenericService<QuestionnaireDto, Questionnaire>
    {
    }
}
