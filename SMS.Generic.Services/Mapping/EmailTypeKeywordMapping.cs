using SMS.Generic.Common.Helpers;
using SMS.Generic.Dtos.Dtos;
using SMS.Generic.Enums;

namespace SMS.Generic.Services.Mapping;

/// <summary>
/// Centralized mapping between <see cref="EmailType"/> and supported template keywords.
/// </summary>
public static class EmailTypeKeywordMapping
{
    private static readonly IReadOnlyDictionary<EmailType, IReadOnlyList<string>> KeywordMap =
        new Dictionary<EmailType, IReadOnlyList<string>>
        {
            [EmailType.CancelledEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.CancellationDate),
                EnumHelper.ToStringValue(EmailKeyword.CancellationTime),
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.CancelledBy),
                EnumHelper.ToStringValue(EmailKeyword.CancelledByRank),
                EnumHelper.ToStringValue(EmailKeyword.CancelledByEmployeeType),
                EnumHelper.ToStringValue(EmailKeyword.CancellationReason)
            },
            [EmailType.ClosePostEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.ClosePostDate),
                EnumHelper.ToStringValue(EmailKeyword.ClosePostTime),
                EnumHelper.ToStringValue(EmailKeyword.ClosedByName),
                EnumHelper.ToStringValue(EmailKeyword.ClosedByRank),
                EnumHelper.ToStringValue(EmailKeyword.ClosedByEmployeeType)
            },
            [EmailType.ApprovalEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.FullName),
                EnumHelper.ToStringValue(EmailKeyword.ApprovalLink),
                EnumHelper.ToStringValue(EmailKeyword.RequestName)
            },
            [EmailType.AssignPostEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeType),
                EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeRank)
            },
            [EmailType.SplitPostEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.SplitDate),
                EnumHelper.ToStringValue(EmailKeyword.SplitTime),
                EnumHelper.ToStringValue(EmailKeyword.SplitBy),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeType),
                EnumHelper.ToStringValue(EmailKeyword.Vehicles),
                EnumHelper.ToStringValue(EmailKeyword.Equipment)
            },
            [EmailType.PostAssignmentAcceptedEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.AcceptedDate),
                EnumHelper.ToStringValue(EmailKeyword.AcceptedTime),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeType)
            },
            [EmailType.PostAssignmentRejectedEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.RejectedDate),
                EnumHelper.ToStringValue(EmailKeyword.RejectedTime),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeType)
            },
            [EmailType.PostSplitConfirmationEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.SplitDate),
                EnumHelper.ToStringValue(EmailKeyword.SplitTime),
                EnumHelper.ToStringValue(EmailKeyword.SplitBy),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeType),
                EnumHelper.ToStringValue(EmailKeyword.Vehicles),
                EnumHelper.ToStringValue(EmailKeyword.Equipment)
            },
            [EmailType.ForgotPasswordOtpEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Otp),
                EnumHelper.ToStringValue(EmailKeyword.FullName),
                EnumHelper.ToStringValue(EmailKeyword.UserName),
                EnumHelper.ToStringValue(EmailKeyword.ExpiryMinutes),
                EnumHelper.ToStringValue(EmailKeyword.DepartmentName)
            },
            [EmailType.EmployeePasswordResetByAdminEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.NewPassword),
                EnumHelper.ToStringValue(EmailKeyword.Password),
                EnumHelper.ToStringValue(EmailKeyword.Employee),
                EnumHelper.ToStringValue(EmailKeyword.FullName),
                EnumHelper.ToStringValue(EmailKeyword.UserName),
                EnumHelper.ToStringValue(EmailKeyword.DepartmentName)
            },
            [EmailType.PostTransferEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.PostEndTime),
                EnumHelper.ToStringValue(EmailKeyword.TransferFrom),
                EnumHelper.ToStringValue(EmailKeyword.TransferTo),
                EnumHelper.ToStringValue(EmailKeyword.TransferEffectiveDate),
                EnumHelper.ToStringValue(EmailKeyword.TransferEffectiveTime),
                EnumHelper.ToStringValue(EmailKeyword.TransferEffectiveTimestamp),
                EnumHelper.ToStringValue(EmailKeyword.SplitBy),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.Vehicles),
                EnumHelper.ToStringValue(EmailKeyword.Equipment)
            },
            [EmailType.PostScheduleUpdatedEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.OldStartTime),
                EnumHelper.ToStringValue(EmailKeyword.NewStartTime),
                EnumHelper.ToStringValue(EmailKeyword.OldEndTime),
                EnumHelper.ToStringValue(EmailKeyword.NewEndTime),
                EnumHelper.ToStringValue(EmailKeyword.OldMinimumHoursRequired),
                EnumHelper.ToStringValue(EmailKeyword.NewMinimumHoursRequired),
                EnumHelper.ToStringValue(EmailKeyword.OldDuration),
                EnumHelper.ToStringValue(EmailKeyword.NewDuration),
                EnumHelper.ToStringValue(EmailKeyword.OldPostLocation),
                EnumHelper.ToStringValue(EmailKeyword.NewPostLocation),
                EnumHelper.ToStringValue(EmailKeyword.VehicleChangeSummary)
            },
            [EmailType.CancelledJobRefilledEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeType),
                EnumHelper.ToStringValue(EmailKeyword.AssignedEmployeeRank)
            },
            [EmailType.PostTransferRolledBackEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.PostEndTime),
                EnumHelper.ToStringValue(EmailKeyword.TransferFrom),
                EnumHelper.ToStringValue(EmailKeyword.TransferTo),
                EnumHelper.ToStringValue(EmailKeyword.TransferFailureReason),
                EnumHelper.ToStringValue(EmailKeyword.SplitBy),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName)
            },
            [EmailType.EmployeeEmailUnavailable] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.FullName),
                EnumHelper.ToStringValue(EmailKeyword.UserName),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeNumber),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeType),
                EnumHelper.ToStringValue(EmailKeyword.NotificationContext)
            },
            [EmailType.JobsPostedEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.RequestName),
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.FullName)
            },
            [EmailType.JobStartingIn24HoursEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName)
            },
            [EmailType.JobStartingIn1HourEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName)
            },
            [EmailType.PostVehicleEquipmentUpdatedEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.PostAssignedDate),
                EnumHelper.ToStringValue(EmailKeyword.PostStartTime),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.Vehicles),
                EnumHelper.ToStringValue(EmailKeyword.Equipment)
            },
            [EmailType.RequestReleasedEmail] = new[]
            {
                EnumHelper.ToStringValue(EmailKeyword.RequestName),
                EnumHelper.ToStringValue(EmailKeyword.Post),
                EnumHelper.ToStringValue(EmailKeyword.EmployeeName),
                EnumHelper.ToStringValue(EmailKeyword.FullName)
            }
        };

    private static readonly IReadOnlyDictionary<EmailType, string> EmailTypeLabels =
        new Dictionary<EmailType, string>
        {
            [EmailType.CancelledEmail] = "Cancelled Email",
            [EmailType.ClosePostEmail] = "Close Post Email",
            [EmailType.ApprovalEmail] = "Approval Email",
            [EmailType.AssignPostEmail] = "Assign Post Email",
            [EmailType.SplitPostEmail] = "Split Post Email",
            [EmailType.PostAssignmentAcceptedEmail] = "Post Assignment Accepted Email",
            [EmailType.PostAssignmentRejectedEmail] = "Post Assignment Rejected Email",
            [EmailType.PostSplitConfirmationEmail] = "Post Split Confirmation Email",
            [EmailType.ForgotPasswordOtpEmail] = "Forgot Password OTP Email",
            [EmailType.EmployeePasswordResetByAdminEmail] = "Employee Password Reset By Admin Email",
            [EmailType.PostTransferEmail] = "Post Transfer Email",
            [EmailType.PostScheduleUpdatedEmail] = "Post Schedule Updated Email",
            [EmailType.CancelledJobRefilledEmail] = "Cancelled Job Refilled Email",
            [EmailType.PostTransferRolledBackEmail] = "Post Transfer Rolled Back Email",
            [EmailType.EmployeeEmailUnavailable] = "Employee Email Unavailable Notification",
            [EmailType.JobsPostedEmail] = "Jobs Posted Email",
            [EmailType.JobStartingIn24HoursEmail] = "Job Starting In 24 Hours Email",
            [EmailType.JobStartingIn1HourEmail] = "Job Starting In 1 Hour Email",
            [EmailType.PostVehicleEquipmentUpdatedEmail] = "Post Vehicle Equipment Updated Email",
            [EmailType.RequestReleasedEmail] = "Request Released Email"
        };

    /// <summary>
    /// Gets the display label for an email type (used in unavailable-notification context).
    /// </summary>
    public static string GetEmailTypeLabel(EmailType emailType) =>
        EmailTypeLabels.TryGetValue(emailType, out var label) ? label : emailType.ToString();

    /// <summary>
    /// Gets distinct keyword names supported by an email type.
    /// </summary>
    public static IReadOnlyList<string> GetKeywordsByEmailType(EmailType emailType)
    {
        if (!KeywordMap.TryGetValue(emailType, out var keywords))
            return Array.Empty<string>();

        return keywords
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// Gets all email types that have keyword mapping metadata, in enum value order, with display labels.
    /// </summary>
    public static IReadOnlyList<EmailTypeOptionDto> GetEmailTypes()
    {
        return KeywordMap.Keys
            .OrderBy(e => (int)e)
            .Select(e => new EmailTypeOptionDto
            {
                Value = (int)e,
                Label = GetEmailTypeLabel(e)
            })
            .ToList();
    }
}
