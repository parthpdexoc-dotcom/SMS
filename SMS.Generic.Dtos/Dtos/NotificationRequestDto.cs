namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request DTO for sending push notifications to multiple devices via FCM.
    /// </summary>
    public class NotificationRequestDto
    {
        /// <summary>
        /// FCM device tokens (one per device).
        /// </summary>
        public List<string> DeviceTokens { get; set; } = new();

        /// <summary>
        /// Notification title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Notification body text.
        /// </summary>
        public string Body { get; set; } = string.Empty;
    }
}
