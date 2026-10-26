namespace SMS.Generic.Dtos.Dtos
{
    public class OtpVerificationResponseDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
