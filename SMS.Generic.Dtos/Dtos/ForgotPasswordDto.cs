namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for forgot password (single API: request OTP or verify OTP and set new password).
    /// Step 1: send only Email → OTP is sent to email and stored.
    /// Step 2: send Email + Otp + NewPassword → OTP is verified, password is set, OTP is cleared.
    /// </summary>
    public class ForgotPasswordDto : LoginDto
    {
        /// <summary>
        /// Gets or sets the OTP received by email. Required only for step 2 (reset password).
        /// </summary>
        public string? Otp { get; set; }

        /// <summary>
        /// Gets or sets the new password. Required only for step 2 (reset password).
        /// </summary>
        public string? NewPassword { get; set; }
    }
}

