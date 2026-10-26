using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Entities.Entities;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Interface for admin service operations.
    /// </summary>
    public interface IAdminService : IGenericService<AdminDto, Admin>
    {
        /// <summary>
        /// Authenticates an admin and generates a JWT token.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>The token DTO if authentication is successful; otherwise, null.</returns>
        Task<TokenDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Generates a new access token from the admin's current persisted state.
        /// </summary>
        Task<TokenDto?> GenerateAccessTokenAsync(Guid adminId);

        /// <summary>
        /// Initiates password recovery process for an admin.
        /// </summary>
        /// <param name="forgotPasswordDto">The password recovery request.</param>
        /// <returns>The password recovery response with login ID and email confirmation.</returns>
        Task<PasswordRecoveryResponseDto?> ForgotPasswordAsync(AdminForgotPasswordDto forgotPasswordDto);

        /// <summary>
        /// Loads an admin, applies a JSON Patch document (replace-only), and returns the resulting DTO for validation and upsert.
        /// Does not persist or run FluentValidation; callers validate then call <see cref="IGenericService{AdminDto, Admin}.UpsertAsync"/>.
        /// </summary>
        /// <param name="id">The admin identifier from the route.</param>
        /// <param name="patchDoc">The JSON Patch document describing the changes.</param>
        /// <returns>The patched admin DTO, or null if the admin was not found.</returns>
        Task<AdminDto?> BuildPatchedAdminAsync(Guid id, Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<AdminDto> patchDoc);
    }
}

