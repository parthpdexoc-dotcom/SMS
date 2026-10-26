using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester locations service operations.
    /// </summary>
    public interface IRequesterLocationsService : IGenericService<RequesterLocationsDto, RequesterLocations>
    {
        /// <summary>
        /// Retrieves a requester location by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester location DTO if found; otherwise, null.</returns>
        Task<RequesterLocationsDto?> GetByIdAsync(long id);
    }
}

