using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for posts on duty service operations.
    /// </summary>
    public interface IPostsOnDutyService : IGenericService<PostsOnDutyDto, PostsOnDuty>
    {
        /// <summary>
        /// Retrieves a posts on duty by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The posts on duty DTO if found; otherwise, null.</returns>
        Task<PostsOnDutyDto?> GetByIdAsync(int id);
    }
}

