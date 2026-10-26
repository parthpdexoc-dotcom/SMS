using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Repositories.Interfaces
{
    /// <summary>
    /// Interface for admin repository operations, extending IGenericRepository.
    /// </summary>
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        /// <summary>
        /// Retrieves an admin with all related details (AdminType) for login operations.
        /// Uses a single optimized query with LINQ joins to fetch all required data.
        /// </summary>
        /// <param name="username">The username of the admin.</param>
        /// <returns>The admin login information DTO if found; otherwise, null.</returns>
        Task<AdminLoginInfoDto?> GetAdminWithDetailsForLoginAsync(string username);

        /// <summary>
        /// Retrieves current admin details required to rebuild an access token.
        /// </summary>
        Task<AdminLoginInfoDto?> GetAdminWithDetailsByIdAsync(Guid adminId);

        /// <summary>
        /// Updates the password for an admin.
        /// </summary>
        /// <param name="adminId">The admin ID.</param>
        /// <param name="hashedPassword">The hashed password to set.</param>
        /// <returns>True if the password was updated successfully; otherwise, false.</returns>
        Task<bool> UpdatePasswordAsync(Guid adminId, string hashedPassword);

        /// <summary>
        /// Retrieves an admin by username.
        /// </summary>
        /// <param name="username">The username of the admin.</param>
        /// <returns>The admin entity if found; otherwise, null.</returns>
        Task<Admin?> GetByUsernameAsync(string username);
    }
}

