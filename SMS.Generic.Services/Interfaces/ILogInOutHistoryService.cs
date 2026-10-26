using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for login/logout history service operations.
    /// </summary>
    public interface ILogInOutHistoryService : IGenericService<LogInOutHistoryDto, LogInOutHistory>
    {
        /// <summary>
        /// Retrieves a login/logout history record by its primary key (long).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The login/logout history DTO if found; otherwise, null.</returns>
        Task<LogInOutHistoryDto?> GetByIdAsync(long id);
    }
}

