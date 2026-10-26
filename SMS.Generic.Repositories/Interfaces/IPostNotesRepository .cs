using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for post repository operations, extending IGenericRepository.
    /// </summary>
    public interface IPostNotesRepository : IGenericRepository<PostNotes>
    {
        // Add post-specific repository methods here if needed
        // Otherwise, leave empty - generic methods are inherited
    }
}

