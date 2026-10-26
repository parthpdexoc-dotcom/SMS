using SMS.Generic.Common.Implementations.Paginations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for notification data response.
    /// Supports three scenarios:
    /// 1. Request level: child posts with their assigned employees
    /// 2. Post level: assigned employees for a specific post
    /// 3. Employee level: single employee data
    /// </summary>
    public class NotificationDataDto
    {
        public Guid? RequestId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? EmployeeId { get; set; }

        /// <summary>Requester display name for modal header.</summary>
        public string? RequesterName { get; set; }

        /// <summary>Pre-populated subject line for the notification composer.</summary>
        public string? SuggestedSubject { get; set; }

        /// <summary>Post title (post and employee level).</summary>
        public string? PostTitle { get; set; }

        /// <summary>Post location (post and employee level).</summary>
        public string? PostLocation { get; set; }

        /// <summary>Human-readable post code, e.g. 000001 (post and employee level).</summary>
        public string? PostCode { get; set; }

        /// <summary>Child posts (request level).</summary>
        public List<NotificationPostDto>? Posts { get; set; }

        /// <summary>Assigned employees for the post or request (post and request level).</summary>
        public PagedList<NotificationEmployeeDto>? AssignedEmployees { get; set; }

        /// <summary>Single employee recipient (employee level).</summary>
        public NotificationEmployeeDto? Employee { get; set; }

        /// <summary>Suggested additional recipients (requester, request, post contact emails). Admin picks To/Cc/Bcc in UI.</summary>
        public List<NotificationSuggestedRecipientDto>? SuggestedRecipients { get; set; }

        /// <summary>Builds the suggested subject for a post-level notification.</summary>
        public static string BuildPostSuggestedSubject(string? postTitle, string? postCode)
        {
            var jobLabel = string.IsNullOrWhiteSpace(postCode) ? postTitle : $"{postTitle} - Job #{postCode}";
            return $"Notification: {jobLabel}";
        }

        /// <summary>Builds the suggested subject for a request-level notification.</summary>
        public static string BuildRequestSuggestedSubject(string? requestTitle, string? requestCode)
        {
            var label = string.IsNullOrWhiteSpace(requestTitle) ? "Request" : requestTitle;
            if (!string.IsNullOrWhiteSpace(requestCode))
                label += $" - Job #{requestCode}";
            return $"Notification: {label}";
        }
    }
}
