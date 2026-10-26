namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for password recovery response.
    /// </summary>
    public class PasswordRecoveryResponseDto
    {
        /// <summary>
        /// Gets or sets the login ID (username).
        /// </summary>
        public string LoginId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a message indicating the password has been sent to email.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address where the password was sent.
        /// </summary>
        public string EmailAddress { get; set; } = string.Empty;
    }
}

