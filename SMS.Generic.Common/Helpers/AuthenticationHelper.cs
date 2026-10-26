using Microsoft.Extensions.Logging;
using SMS.Generic.Common.Enums;

namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Helper class for common authentication operations (password validation and hashing).
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Validates a password against stored password (handles both hashed and plain text for backward compatibility).
        /// If password is plain text and valid, it will be hashed and updated in the database.
        /// </summary>
        /// <param name="loginPassword">The password from login request.</param>
        /// <param name="storedPassword">The password stored in database.</param>
        /// <param name="userId">The user ID (for updating password if needed).</param>
        /// <param name="updatePasswordFunc">Function to update password in database (returns true if successful).</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="userType">The type of user for logging.</param>
        /// <returns>True if password is valid; otherwise, false.</returns>
        public static async Task<bool> ValidateAndHashPasswordIfNeededAsync(
            string loginPassword,
            string? storedPassword,
            Guid userId,
            Func<Guid, string, Task<bool>> updatePasswordFunc,
            ILogger logger,
            UserType userType = UserType.Employee)
        {
            if (string.IsNullOrWhiteSpace(storedPassword))
            {
                logger.LogWarning("Stored password is empty for {UserType} ID: {UserID}", userType.ToString(), userId);
                return false;
            }

            // Check if password is already hashed
            bool isPasswordHashed = PasswordHelper.IsPasswordHashed(storedPassword);
            bool isPasswordValid = false;

            if (isPasswordHashed)
            {
                // Password is already hashed, verify it
                isPasswordValid = PasswordHelper.VerifyPassword(loginPassword, storedPassword);
            }
            else
            {
                // Password is plain text (legacy), compare directly and then hash it
                if (storedPassword == loginPassword)
                {
                    isPasswordValid = true;
                    // Hash the password and update it in the database
                    var hashedPassword = PasswordHelper.HashPassword(loginPassword);
                    var updated = await updatePasswordFunc(userId, hashedPassword);
                    if (updated)
                    {
                        logger.LogInformation("Password hashed and updated for {UserType} ID: {UserID}", userType.ToString(), userId);
                    }
                    else
                    {
                        logger.LogWarning("Failed to update password for {UserType} ID: {UserID}", userType.ToString(), userId);
                    }
                }
            }

            return isPasswordValid;
        }
    }
}

