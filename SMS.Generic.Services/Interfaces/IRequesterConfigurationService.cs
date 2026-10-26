using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester configuration service operations.
    /// </summary>
    public interface IRequesterConfigurationService : IGenericService<RequesterConfigurationDto, RequesterConfiguration>
    {
        /// <summary>
        /// Retrieves a requester configuration by its primary key (bigint/long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester configuration DTO if found; otherwise, null.</returns>
        Task<RequesterConfigurationDto?> GetByIdAsync(long id);
    }
}

