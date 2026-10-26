namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for JWT token response.
    /// </summary>
    public class TokenDto
    {
        /// <summary>
        /// Parameterless constructor for serialization / object initializer usage.
        /// </summary>
        public TokenDto() { }

        /// <summary>
        /// Creates a token response with the given values.
        /// </summary>
        /// <param name="token">JWT access token.</param>
        /// <param name="expiresAt">Token expiration date and time.</param>
        /// <param name="tokenType">Token type (default "Bearer").</param>
        /// <param name="deviceToken">Optional device token for push notifications.</param>
        /// <param name="isPasswordReset">Indicates password reset is required before issuing a token.</param>
        public TokenDto(string? token, DateTime expiresAt, string tokenType = "Bearer", string? deviceToken = null, bool isPasswordReset = false)
        {
            Token = token;
            ExpiresAt = expiresAt;
            TokenType = tokenType;
            DeviceToken = deviceToken ?? string.Empty;
            IsPasswordReset = isPasswordReset;
        }

        /// <summary>
        /// Gets or sets the JWT access token.
        /// </summary>
        public string? Token { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token expiration date and time.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the token type (typically "Bearer").
        /// </summary>
        public string TokenType { get; set; } = "Bearer";
        /// <summary>
        /// Gets or sets the device token.
        /// </summary>
        public string DeviceToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the user must reset password before login token can be issued.
        /// </summary>
        public bool IsPasswordReset { get; set; }

    }
}

