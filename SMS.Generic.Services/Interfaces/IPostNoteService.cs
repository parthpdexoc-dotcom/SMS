using SMS.Generic.Common.Implementations.Paginations;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for post service operations.
    /// </summary>
    public interface IPostNoteService : IGenericService<PostNotesDto, PostNotes>
    {
        /// <summary>
        /// Gets the latest note for a specific post ID where IsLatest is true.
        /// </summary>
        /// <param name="postId">The post ID.</param>
        /// <returns>The latest post note if found; otherwise, null.</returns>
        Task<PostNotesDto?> GetLatestNoteByPostIdAsync(Guid postId);
    }
}

