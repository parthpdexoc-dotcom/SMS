using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for request service operations.
    /// </summary>
    public interface IRequestNoteService : IGenericService<RequestNotesDto, RequestNotes>
    {
        /// <summary>
        /// Gets the latest note for a specific request ID where IsLatest is true.
        /// </summary>
        /// <param name="requestId">The request ID.</param>
        /// <returns>The latest request note if found; otherwise, null.</returns>
        Task<RequestNotesDto?> GetLatestNoteByRequestIdAsync(Guid requestId);
    }
}

