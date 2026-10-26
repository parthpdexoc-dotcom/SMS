using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for image details service operations.
    /// </summary>
    public interface IImageDetailsService : IGenericService<ImageDetailsDto, ImageDetails>
    {
        /// <summary>
        /// Retrieves an image detail by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The image detail DTO if found; otherwise, null.</returns>
        Task<ImageDetailsDto?> GetByIdAsync(int id);
    }
}

