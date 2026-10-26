namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Response DTO for send-multiple push notification.
    /// </summary>
    public class SendNotificationResultDto
    {
        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        public SendNotificationResultDto() { }

        /// <summary>
        /// Creates a send notification result with the given counts and failed tokens.
        /// </summary>
        public SendNotificationResultDto(int successCount, int failureCount, IReadOnlyList<string>? failedTokens = null)
        {
            SuccessCount = successCount;
            FailureCount = failureCount;
            FailedTokens = failedTokens?.ToList() ?? new List<string>();
        }

        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> FailedTokens { get; set; } = new();
    }
}
