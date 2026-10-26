using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for requester contacts service operations.
    /// </summary>
    public interface IRequesterContactsService : IGenericService<RequesterContactsDto, RequesterContacts>
    {
        /// <summary>
        /// Retrieves a requester contact by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The requester contact DTO if found; otherwise, null.</returns>
        Task<RequesterContactsDto?> GetByIdAsync(long id);
    }
}

