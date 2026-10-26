using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for users service operations.
    /// </summary>
    public interface IUsersService : IGenericService<UsersDto, Users>
    {
        /// <summary>
        /// Retrieves a user by its primary key (int).
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <returns>The user DTO if found; otherwise, null.</returns>
        Task<UsersDto?> GetByIdAsync(int id);
    }
}

