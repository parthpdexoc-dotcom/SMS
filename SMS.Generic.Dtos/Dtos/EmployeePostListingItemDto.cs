using Newtonsoft.Json;
using SMS.Generic.Common.Enums;
using SMS.Generic.Enums;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Unified list-row shape for employee post flows (available posts, assignments, waiting list, closed history)
    /// so clients can use one model across related APIs.
    /// </summary>
    public class EmployeePostListingItemDto 
    {
        // Request / post (catalog). Title/Location follow IsSpecialEvent rules in listing mappers.
        public Guid? RequestID { get; set; }
        public Guid? PostID { get; set; }
        /// <summary>When <see cref="IsSpecialEvent"/> is true, copied from the request title; otherwise from the post title.</summary>
        public string? Title { get; set; }
        /// <summary>When <see cref="IsSpecialEvent"/> is true, copied from the request location; otherwise from the post location.</summary>
        public string? Location { get; set; }
        public string? FailureReason { get; set; }
        public int? PostLength { get; set; }
        public int? TotalPositions { get; set; }
        public int? AvailablePositions { get; set; }

        /// <summary>Set for open-release rows that represent a special-event aggregate (see <see cref="AvailablePostDto.IsSpecialEvent"/>).</summary>
        public bool? IsSpecialEvent { get; set; }

        public ReleaseAs ReleaseAs { get; set; }

        // Assignment (post position)
        [JsonProperty("postPositionId")]
        [JsonPropertyName("postPositionId")]
        public Guid? PostPositionID { get; set; }
        public PostPositionStatus? PostPositionStatus { get; set; }
        /// <summary>Name of the employee (from <c>PostPosition.EmployeeID</c>) for cancelled or cancellation-pending positions.</summary>
        public string? CancelledBy { get; set; }
        public Guid? ParentPostPositionId { get; set; }
        public bool? IsSplit { get; set; }
        public Guid? SplitPostPositionID { get; set; }
        public bool? ShowVehicle { get; set; }
        public bool? ShowEquipment { get; set; }
        public Guid? EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        // ✅ Unified fields
        public string? RequestTitle { get; set; }
        public Guid? RequesterID { get; set; }
        public string? RequesterTitle { get; set; }
        public string? PayCodeValue { get; set; }
        /// <summary>Rank of the job from the post's employee type abbreviation (e.g. LT, SGT, OFC).</summary>
        public string? JobRank { get; set; }
        public DateOnly? StartDate { get; set; }
        public string? DayOfWeek => StartDate.HasValue
            ? StartDate.Value.ToDateTime(TimeOnly.MinValue).ToString("ddd").ToUpperInvariant()
            : null;
        public TimeOnly? StartTime { get; set; }
        /// <summary>Post catalog end time (from Post / listing row); UI should not derive from start + duration.</summary>
        public TimeOnly? PostEndTime { get; set; }
        public TimeOnly? WorkStartTime { get; set; }
        public TimeOnly? WorkEndTime { get; set; }

        [JsonProperty("waitingPostPositionId")]
        [JsonPropertyName("waitingPostPositionId")]
        public Guid? WaitingPostPositionID { get; set; }

        public int? OrderNumber { get; set; }
        /// <summary>
        /// True when the employee has already been applied/assigned from the tenure waiting-list row.
        /// </summary>
        public bool HasAppliedInTenure { get; set; }
        public ProcessingStatus? ProcessingStatus { get; set; }

        public EmployeePostListingItemDto(
            Guid? postId,
            Guid? requestId,
            DateOnly? startDate,
            TimeOnly? startTime,
            TimeOnly? workStartTime,
            TimeOnly? workEndTime,
            int? postLength,
            string? postTitle,
            string? postLocation,
            string? requestTitle,
            string? requestLocation,
            bool isSpecialEvent)
        {
            PostID = postId;
            RequestID = requestId;
            StartDate = startDate;
            StartTime = startTime;
            WorkStartTime = workStartTime;
            WorkEndTime = workEndTime;
            PostLength = postLength;
            Title = isSpecialEvent ? requestTitle : postTitle;
            Location = isSpecialEvent ? requestLocation : postLocation;
            IsSpecialEvent = isSpecialEvent;
        }

        public EmployeePostListingItemDto()
        {
        }

        /// <summary>
        /// Department-local shift start, or null when <see cref="StartDate"/> is missing.
        /// Uses midnight when <see cref="StartTime"/> is missing.
        /// </summary>
        public DateTime? GetShiftStartDateTime()
        {
            if (!StartDate.HasValue)
                return null;

            return StartDate.Value.ToDateTime(StartTime ?? TimeOnly.MinValue);
        }

        /// <summary>
        /// True when the shift start (date and time, if present) is at or before department-local <paramref name="departmentNow"/>.
        /// Without a start time, compares by date only.
        /// </summary>
        public bool IsShiftStartPassed(DateTime departmentNow)
        {
            var shiftStart = GetShiftStartDateTime();
            if (!shiftStart.HasValue)
                return false;

            if (!StartTime.HasValue)
                return StartDate!.Value < DateOnly.FromDateTime(departmentNow);

            return departmentNow >= shiftStart.Value;
        }

        /// <summary>
        /// True when shift start is on or after <paramref name="departmentNow"/> minus <paramref name="lookbackDays"/> (default 7).
        /// </summary>
        public bool IsWithinWaitingListLookbackWindow(DateTime departmentNow, int lookbackDays = 7)
        {
            var shiftStart = GetShiftStartDateTime();
            if (!shiftStart.HasValue)
                return false;

            return shiftStart.Value >= departmentNow.AddDays(-lookbackDays);
        }
    }
}
