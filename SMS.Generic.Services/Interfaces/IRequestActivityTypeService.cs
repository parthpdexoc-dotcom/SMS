using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for request activity type service operations.
    /// </summary>
    public interface IRequestActivityTypeService : IGenericService<RequestActivityTypeDto, RequestActivityType>
    {
        /// <summary>
        /// Retrieves a request activity type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The request activity type DTO if found; otherwise, null.</returns>
        Task<RequestActivityTypeDto?> GetByIdAsync(int id);
    }
}

