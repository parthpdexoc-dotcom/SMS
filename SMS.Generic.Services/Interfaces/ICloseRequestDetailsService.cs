using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for close request details service operations.
    /// </summary>
    public interface ICloseRequestDetailsService : IGenericService<CloseRequestDetailsDto, CloseRequestDetails>
    {
        /// <summary>
        /// Retrieves a close request detail by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The close request detail DTO if found; otherwise, null.</returns>
        Task<CloseRequestDetailsDto?> GetByIdAsync(long id);
    }
}

