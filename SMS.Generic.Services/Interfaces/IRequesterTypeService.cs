using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester type service operations.
    /// </summary>
    public interface IRequesterTypeService : IGenericService<RequesterTypeDto, RequesterType>
    {
        /// <summary>
        /// Retrieves a requester type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester type DTO if found; otherwise, null.</returns>
        Task<RequesterTypeDto?> GetByIdAsync(int id);
    }
}

