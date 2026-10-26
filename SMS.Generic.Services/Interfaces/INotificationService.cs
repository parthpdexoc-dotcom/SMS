using Microsoft.AspNetCore.Http;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Interfaces
{
    /// <summary>
    /// Service for send-notification and notification modal data operations.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Gets Send Notification modal data. Exactly one of postPositionId, postId, or requestId must be provided.
        /// </summary>
        Task<NotificationDataDto> GetNotificationDataAsync(
            NotificationDataRequestDto request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Parses and sends a notification email from an HTTP request (JSON or multipart/form-data).
        /// </summary>
        Task SendNotificationAsync(HttpRequest httpRequest, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends jobs-posted or request-released emails to subscribed employees in the caller's department (from token)
        /// when <paramref name="notifyWhenJobsPosted"/> is true.
        /// Job-start reminders are handled by the cron job.
        /// </summary>
        Task SendEmployeePreferenceNotificationsAsync(
            bool notifyWhenJobsPosted,
            Guid postId,
            EmailType emailType = EmailType.JobsPostedEmail,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a job-start reminder email to an employee, or notifies admins when email is unavailable.
        /// </summary>
        Task SendJobStartingReminderEmailAsync(
            EmployeeDto employee,
            PostPositionDto postPosition,
            EmailType emailType,
            CancellationToken cancellationToken = default);
    }
}
