using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for question service operations.
    /// </summary>
    public interface IQuestionService : IGenericService<QuestionDto, Question>
    {
    }
}
