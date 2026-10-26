using SMS.Generic.Dtos.Dtos;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service for sending Firebase Cloud Messaging (FCM) push notifications to mobile devices.
    /// </summary>
    public interface IFirebasePushNotificationService
    {
        /// <summary>
        /// Sends a notification to multiple devices by their FCM tokens.
        /// </summary>
        /// <param name="request">Title, body, and list of device tokens.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>Success count, failure count, and list of failed tokens (e.g. for removal from DB).</returns>
        Task<(int SuccessCount, int FailureCount, IReadOnlyList<string> FailedTokens)> SendToMultipleDevicesAsync(
            NotificationRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Resolves device tokens for the given employee IDs and sends a push notification to those devices.
        /// Best-effort: logs and swallows errors so callers are not affected. Use from bulk assign, single assign, etc.
        /// </summary>
        /// <param name="employeeIds">Employee IDs to send the notification to.</param>
        /// <param name="title">Notification title.</param>
        /// <param name="body">Notification body.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        Task SendToEmployeesAsync(
            IReadOnlyList<Guid> employeeIds,
            string title,
            string body,
            CancellationToken cancellationToken = default);
    }
}
