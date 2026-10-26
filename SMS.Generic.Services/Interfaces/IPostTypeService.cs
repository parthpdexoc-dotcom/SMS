using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for post type service operations.
    /// </summary>
    public interface IPostTypeService : IGenericService<PostTypeDto, PostType>
    {
        /// <summary>
        /// Retrieves a post type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The post type DTO if found; otherwise, null.</returns>
        Task<PostTypeDto?> GetByIdAsync(int id);
    }
}

