using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SMS.Generic.Common.Helpers
{
    /// <summary>
    /// Helper class for password validation and hashing operations.
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Validates that a password meets the system requirements.
        /// Password must be at least 6 characters and contain:
        /// - At least one uppercase letter (A-Z)
        /// - At least one lowercase letter (a-z)
        /// - At least one number (0-9)
        /// - At least one special character
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <returns>True if the password is valid; otherwise, false.</returns>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // Must be at least 6 characters
            if (password.Length < 6)
                return false;

            // Check for at least one uppercase letter
            if (!password.Any(c => char.IsUpper(c)))
                return false;

            // Check for at least one lowercase letter
            if (!password.Any(c => char.IsLower(c)))
                return false;

            // Check for at least one number
            if (!password.Any(c => char.IsDigit(c)))
                return false;

            // Check for at least one special character (non-alphanumeric)
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if a password is already hashed using BCrypt algorithm.
        /// BCrypt hashes start with $2a$, $2b$, or $2y$.
        /// </summary>
        /// <param name="password">The password string to check.</param>
        /// <returns>True if the password is already hashed; otherwise, false.</returns>
        public static bool IsPasswordHashed(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // BCrypt hashes start with $2a$, $2b$, or $2y$
            return password.StartsWith("$2a$", StringComparison.Ordinal) ||
                   password.StartsWith("$2b$", StringComparison.Ordinal) ||
                   password.StartsWith("$2y$", StringComparison.Ordinal);
        }

        /// <summary>
        /// Hashes a password using BCrypt algorithm.
        /// If the password is already hashed, returns it unchanged.
        /// </summary>
        /// <param name="password">The plain text password to hash.</param>
        /// <returns>The BCrypt hashed password.</returns>
        /// <exception cref="ArgumentException">Thrown when password is null or empty.</exception>
        public static string HashPassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            // If already hashed, return as-is
            if (IsPasswordHashed(password))
                return password;

            // Hash the plain text password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Generates a long, cryptographically secure random password suitable for system accounts
        /// that should not be login-capable (e.g. Master employees). Uses max practical length for BCrypt (72 bytes).
        /// </summary>
        /// <param name="length">Password length (default 72, max for BCrypt input).</param>
        /// <returns>A cryptographically secure random password with upper, lower, digits, and special chars.</returns>
        public static string GenerateSecureRandomPassword(int length = 72)
        {
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";
            var charset = upper + lower + digits + special;

            var bytes = RandomNumberGenerator.GetBytes(length);
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                result.Append(charset[bytes[i] % charset.Length]);
            return result.ToString();
        }

        /// <summary>
        /// Verifies a plain text password against a BCrypt hash.
        /// </summary>
        /// <param name="plainTextPassword">The plain text password to verify.</param>
        /// <param name="hashedPassword">The BCrypt hashed password to verify against.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        /// <exception cref="ArgumentException">Thrown when either parameter is null or empty.</exception>
        public static bool VerifyPassword(string? plainTextPassword, string? hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(plainTextPassword))
                throw new ArgumentException("Plain text password cannot be null or empty", nameof(plainTextPassword));

            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("Hashed password cannot be null or empty", nameof(hashedPassword));

            // Verify the password using BCrypt
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, hashedPassword);
        }
    }
}

