using System.ComponentModel;

namespace SMS.Generic.Enums
{
    /// <summary>
    /// Template placeholder keys for email templates. Values match [Keyword] placeholders in templates.
    /// Keep in sync with UI (e.g. EMAIL_TEMPLATES.DEFAULT_KEYWORDS) and backend replacement logic.
    /// Enum names use PascalCase; templates may use spaces (e.g. [Cancelled By]) - use explicit string when building parameters for those.
    /// </summary>
    public enum EmailKeyword
    {
        /// <summary>[FullName]</summary>
        FullName,

        /// <summary>[NewPassword]</summary>
        NewPassword,

        /// <summary>[Password]</summary>
        Password,

        /// <summary>[Employee]</summary>
        Employee,

        /// <summary>[UserName]</summary>
        UserName,

        /// <summary>[OTP]</summary>
        [Description("OTP")]
        Otp,

        /// <summary>[ExpiryMinutes]</summary>
        ExpiryMinutes,

        /// <summary>[DepartmentName]</summary>
        DepartmentName,

        /// <summary>[Cancelled By] - Name of employee who cancelled (cancellation emails).</summary>
        [Description("Cancelled By")]
        CancelledBy,

        /// <summary>[cancelledByRank] - Rank of employee who cancelled (cancellation emails).</summary>
        [Description("cancelledByRank")]
        CancelledByRank,

        /// <summary>[cancelledByEmployeeType] - Employee type of who cancelled (cancellation emails).</summary>
        [Description("cancelledByEmployeeType")]
        CancelledByEmployeeType,

        /// <summary>[CancellationDate] - Replaced with post position's date of work (e.g. for cancellation emails).</summary>
        CancellationDate,

        /// <summary>[CancellationTime] - Replaced with post position's start time (e.g. for cancellation emails).</summary>
        CancellationTime,

        /// <summary>[CancellationReason] - Replaced with cancellation reason text.</summary>
        CancellationReason,

        /// <summary>[Post] - Replaced with post title or post details (e.g. for cancellation emails).</summary>
        Post,

        /// <summary>[ApprovalLink] - Replaced with the vehicle/approval link URL (e.g. for vehicle approval emails).</summary>
        ApprovalLink,

        /// <summary>[RequestName] - Replaced with the request name / requester display name (e.g. for vehicle approval emails).</summary>
        RequestName,

        /// <summary>[PostAssignedDate] - Replaced with the assigned post's date of work (e.g. for assign post emails).</summary>
        PostAssignedDate,

        /// <summary>[PostStartTime] - Replaced with the assigned post's start time (e.g. for assign post emails).</summary>
        PostStartTime,

        PostEndTime,

        /// <summary>[AssignedEmployeeName] - Name of employee who took the shift (assign post emails).</summary>
        [Description("AssignedEmployeeName")]
        AssignedEmployeeName,

        /// <summary>[assignedEmployeeRank] - Rank of employee who took the shift (assign post emails).</summary>
        [Description("assignedEmployeeRank")]
        AssignedEmployeeRank,

        /// <summary>[assignedEmployeeType] - Employee type of who took the shift (assign post emails).</summary>
        [Description("assignedEmployeeType")]
        AssignedEmployeeType,

        /// <summary>[ClosePostDate] - Replaced with the close post's date (e.g. for close post emails).</summary>
        ClosePostDate,

        /// <summary>[ClosePostTime] - Replaced with the close post's time (e.g. for close post emails).</summary>
        ClosePostTime,

        /// <summary>[ClosedBy] - Name of employee who closed the post (close post emails).</summary>
        [Description("ClosedBy")]
        ClosedByName,

        /// <summary>[ClosedByRank] - Rank of employee who closed the post (close post emails).</summary>
        [Description("ClosedByRank")]
        ClosedByRank,

        /// <summary>[ClosedByEmployeeType] - Employee type of who closed the post (close post emails).</summary>
        [Description("ClosedByEmployeeType")]
        ClosedByEmployeeType,

        /// <summary>[SplitDate] - Date when the post was split (split assignment emails).</summary>
        SplitDate,

        /// <summary>[SplitTime] - Time when the post was split (split assignment emails).</summary>
        SplitTime,

        /// <summary>[SplitBy] - Name of employee who performed the split (split assignment emails).</summary>
        SplitBy,

        /// <summary>[EmployeeName] - Name of employee assigned to the split (split assignment or accept/reject emails).</summary>
        EmployeeName,

        /// <summary>[EmployeeType] - Employee type of the assigned employee (split assignment or accept/reject emails).</summary>
        EmployeeType,

        /// <summary>[AcceptedDate] - Date when the split assignment was accepted (post assignment accepted emails).</summary>
        AcceptedDate,

        /// <summary>[AcceptedTime] - Time when the split assignment was accepted (post assignment accepted emails).</summary>
        AcceptedTime,

        /// <summary>[RejectedDate] - Date when the split assignment was rejected (post assignment rejected emails).</summary>
        RejectedDate,

        /// <summary>[RejectedTime] - Time when the split assignment was rejected (post assignment rejected emails).</summary>
        RejectedTime,

        /// <summary>[OldStartTime] - Previous scheduled start time before post update confirmation.</summary>
        OldStartTime,

        /// <summary>[NewStartTime] - Updated scheduled start time after post update confirmation.</summary>
        NewStartTime,

        /// <summary>[OldEndTime] - Previous scheduled end time before post update confirmation.</summary>
        OldEndTime,

        /// <summary>[NewEndTime] - Updated scheduled end time after post update confirmation.</summary>
        NewEndTime,

        /// <summary>[OldMinimumHoursRequired] - Previous post minimum hours required before update confirmation.</summary>
        OldMinimumHoursRequired,

        /// <summary>[NewMinimumHoursRequired] - Updated post minimum hours required after confirmation.</summary>
        NewMinimumHoursRequired,

        /// <summary>[OldDuration] - Previous duration value before post update confirmation.</summary>
        OldDuration,

        /// <summary>[NewDuration] - Updated duration value after post update confirmation.</summary>
        NewDuration,

        /// <summary>[OldPostLocation] - Previous post location before schedule update.</summary>
        OldPostLocation,

        /// <summary>[NewPostLocation] - Updated post location after schedule update.</summary>
        NewPostLocation,

        /// <summary>[VehicleChangeSummary] - Vehicle required flag and assignment changes for post schedule update emails.</summary>
        VehicleChangeSummary,

        /// <summary>[TransferFrom] - Source employee name (transfer completed emails).</summary>
        [Description("TransferFrom")]
        TransferFrom,

        /// <summary>[TransferTo] - Recipient employee name (transfer completed emails).</summary>
        [Description("TransferTo")]
        TransferTo,

        /// <summary>[TransferEffectiveDate] - Department-local date when the transfer was completed.</summary>
        TransferEffectiveDate,

        /// <summary>[TransferEffectiveTime] - Department-local time when the transfer was completed.</summary>
        TransferEffectiveTime,

        /// <summary>[TransferEffectiveTimestamp] - Department-local date and time when the transfer was completed.</summary>
        [Description("TransferEffectiveTimestamp")]
        TransferEffectiveTimestamp,

        /// <summary>[TransferFailureReason] - Why the transfer did not complete.</summary>
        TransferFailureReason,

        /// <summary>[Vehicles] - Vehicle details for the job or assignment (comma/semicolon separated).</summary>
        Vehicles,

        /// <summary>[Equipment] - Non-vehicle equipment details for the job or assignment (comma/semicolon separated).</summary>
        Equipment,

        /// <summary>[EmployeeNumber] - Employee number.</summary>
        EmployeeNumber,

        /// <summary>[JobRank] - Minimum job rank required (e.g. LT, SGT, OFC).</summary>
        JobRank,

        /// <summary>[PayCode] - Pay code value for the post (e.g. jobs posted emails).</summary>
        PayCode,

        /// <summary>[NotificationContext] - Description of the notification that could not be delivered (e.g. assign post email).</summary>
        NotificationContext,

    }
}
