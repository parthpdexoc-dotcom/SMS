namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for authentication login operations.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the department code for login.
        /// </summary>
        public string DepartmentCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the username for login.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for login.
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the device token for login.
        /// </summary>
        public string DeviceToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the login client. The web app sends the AES ciphertext of <see cref="SMS.Generic.Common.Constants.ClaimNames.WebLoginClient"/>.
        /// A null or empty value is treated as the web app.
        /// </summary>
        public string? LoginClient { get; set; }
    }
}
