namespace SMS.Generic.Enums
{
    /// <summary>
    /// Represents the type of question in an activity form.
    /// </summary>
    public enum EmailType
    {
        /// <summary>
        /// Short answer question type.
        /// </summary>
        CancelledEmail = 0,

        /// <summary>
        /// Close Post email type.
        /// </summary>
        ClosePostEmail = 1,

        /// <summary>
        /// Approval email type.
        /// </summary>
        ApprovalEmail = 2,

        /// <summary>
        /// Assign Post email (e.g. when an employee is assigned to a post; uses [Post], [PostAssignedDate], [PostStartTime] placeholders).
        /// </summary>
        AssignPostEmail = 4,

        /// <summary>
        /// Split Post email (e.g. when an existing post assignment is split into multiple posts; 
        /// uses [Post], [OriginalPostDate], [OriginalStartTime], [NewPostDate], [NewStartTime] placeholders).
        /// </summary>
        SplitPostEmail = 5,

        /// <summary>
        /// Post Assignment Accepted email type (sent to the splitter when a split post is accepted).
        /// </summary>
        PostAssignmentAcceptedEmail = 7,

        /// <summary>
        /// Post Assignment Rejected email type (sent to the splitter when a split post is rejected).
        /// </summary>
        PostAssignmentRejectedEmail = 8,

        /// <summary>
        /// Post Split Confirmation email type (sent to the splitter immediately after splitting a post).
        /// </summary>
        PostSplitConfirmationEmail = 9,

        /// <summary>
        /// Forgot password OTP email type (sent when employee requests password reset; uses [Otp] placeholder).
        /// </summary>
        ForgotPasswordOtpEmail = 10,

        /// <summary>
        /// Employee password reset by admin (sent when super user or department admin updates an employee password).
        /// </summary>
        EmployeePasswordResetByAdminEmail = 11,

        /// <summary>
        /// Post transfer completed email (sent to the source employee whose post was transferred).
        /// Uses [Post], [PostAssignedDate], [PostStartTime], [PostEndTime], [TransferFrom], [TransferTo],
        /// [TransferEffectiveDate], [TransferEffectiveTime], [TransferEffectiveTimestamp].
        /// </summary>
        PostTransferEmail = 12,

        /// <summary>
        /// Post schedule update email (sent when post schedule, location, or vehicle requirement changes for assigned employees).
        /// </summary>
        PostScheduleUpdatedEmail = 13,

        /// <summary>
        /// Sent to the employee who cancelled (or held) a slot when another employee is assigned to that same slot.
        /// Uses [Post], [PostAssignedDate], [PostStartTime], [AssignedEmployeeName], [assignedEmployeeType], [assignedEmployeeRank].
        /// </summary>
        CancelledJobRefilledEmail = 14,

        /// <summary>
        /// Sent to the transfer initiator when a transfer fails (validation, conflict, or system error).
        /// Uses [Post], [PostAssignedDate], [PostStartTime], [PostEndTime], [TransferFrom], [TransferTo], [TransferFailureReason].
        /// </summary>
        PostTransferRolledBackEmail = 15,

        /// <summary>
        /// Sent to administrators when an employee could not receive an outbound notification because no email is on file.
        /// Uses employee detail placeholders and [NotificationContext].
        /// </summary>
        EmployeeEmailUnavailable = 16,

        /// <summary>
        /// Sent to employees when new jobs are posted/released. Static template; no placeholders.
        /// </summary>
        JobsPostedEmail = 17,

        /// <summary>
        /// Sent to employees when an assigned job is starting in 24 hours.
        /// Uses [Post], [PostAssignedDate], [PostStartTime], [EmployeeName].
        /// </summary>
        JobStartingIn24HoursEmail = 18,

        /// <summary>
        /// Sent to employees when an assigned job is starting in 1 hour.
        /// Uses [Post], [PostAssignedDate], [PostStartTime], [EmployeeName].
        /// </summary>
        JobStartingIn1HourEmail = 19,

        /// <summary>
        /// Sent to employees when a request is released and its jobs are started.
        /// Uses [RequestName], [Post], [EmployeeName], [FullName].
        /// </summary>
        RequestReleasedEmail = 20,
        /// <summary>
        /// Sent to assigned employees on future (not-started) jobs when post-form vehicles/equipment change.
        /// Uses [Post], [PostAssignedDate], [PostStartTime], [EmployeeName], [Vehicles], [Equipment].
        /// </summary>
        PostVehicleEquipmentUpdatedEmail = 25,

     
    }
}
