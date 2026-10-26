using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for request-specific repository operations, extending IGenericRepository.
    /// </summary>
    public interface IRequestNotesRepository : IGenericRepository<RequestNotes>
    {
    }
}

