using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester contact types service operations.
    /// </summary>
    public interface IRequesterContactTypesService : IGenericService<RequesterContactTypesDto, RequesterContactTypes>
    {
        /// <summary>
        /// Retrieves a requester contact type by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester contact type DTO if found; otherwise, null.</returns>
        Task<RequesterContactTypesDto?> GetByIdAsync(int id);
    }
}

