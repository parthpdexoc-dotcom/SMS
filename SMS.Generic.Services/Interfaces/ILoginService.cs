using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for login service operations.
    /// </summary>
    public interface ILoginService : IGenericService<LoginRecordDto, Login>
    {
        /// <summary>
        /// Retrieves a login record by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The login record DTO if found; otherwise, null.</returns>
        Task<LoginRecordDto?> GetByIdAsync(int id);
    }
}

