using SMS.Generic.Dtos.Interfaces;
using SMS.Generic.Enums;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Data Transfer Object for ClosedPostPosition operations.
    /// </summary>
    public class ClosedPostPositionDto : BaseDto
    {
        /// <summary>
        /// Primary Key - Closed Post Position ID (GUID).
        /// </summary>
        public Guid? ClosedPostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Post Position ID.
        /// </summary>
        public Guid? PostPositionID { get; set; }

        /// <summary>
        /// Foreign Key - Employee ID (set from token).
        /// </summary>
        public Guid? SplitEmployeeID { get; set; }

        /// <summary>
        /// Foreign Key - Post ID.
        /// </summary>
        public Guid? PostID { get; set; }

        /// <summary>
        /// Job start time (snapshot from Post).
        /// </summary>
        public TimeOnly PostStartTime { get; set; }

        /// <summary>
        /// Job end time (snapshot from Post).
        /// </summary>
        public TimeOnly PostEndTime { get; set; }

        /// <summary>
        /// Post title from the related post (via <c>Post</c> or <c>PostPosition.Post</c> when loaded).
        /// </summary>
        public string? PostTitle { get; set; }

        /// <summary>
        /// Work start date.
        /// </summary>
        public DateOnly? WorkStartDate { get; set; }

        /// <summary>
        /// Work start time.
        /// </summary>
        public TimeOnly? WorkStartTime { get; set; }

        /// <summary>
        /// Work end time.
        /// </summary>
        public TimeOnly? WorkEndTime { get; set; }

        /// <summary>
        /// Hours worked.
        /// </summary>
        public int? HoursWorked { get; set; }

        /// <summary>
        /// Hours worked on duty.
        /// </summary>
        public bool WorkedOnDuty { get; set; }

        /// <summary>
        /// Comment.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// Requested start date.
        /// </summary>
        public DateOnly? DateOfWork { get; set; }

        /// <summary>
        /// Requested start time.
        /// </summary>
        public TimeOnly? StartTime { get; set; }

        /// <summary>
        /// Requested end time.
        /// </summary>
        public TimeOnly? EndTime { get; set; }

        /// <summary>
        /// Optional JSON alias for <see cref="EndTime"/> (e.g. split flow). Merged into <see cref="EndTime"/> before persist.
        /// </summary>
        [JsonPropertyName("requestedEndTime")]
        public TimeOnly? RequestedEndTimePayload { get; set; }

        /// <summary>
        /// When true, proceed with split despite time-overlap confirmation (same pattern as take-job / assign-to-me).
        /// </summary>
        [JsonProperty("confirmConflict")]
        [JsonPropertyName("confirmConflict")]
        public bool ConfirmConflict { get; set; }

        /// <summary>
        /// User or system that closed the position.
        /// </summary>
        public string? CloseBy { get; set; }

        /// <summary>
        /// Date and time when the position was closed.
        /// </summary>
        public DateTime? CloseDateTime { get; set; }

        /// <summary>
        /// Indicates whether minimum hours are due.
        /// </summary>
        public bool? IsMinimumDue { get; set; }

        /// <summary>
        /// Minimum hours due (when IsMinimumDue is true).
        /// </summary>
        public int? MinimumHoursDue { get; set; }

        /// <summary>
        /// Seniority indicator.
        /// </summary>
        public bool? Seniority { get; set; }

        /// <summary>
        /// Indicates whether this closed post position is related to a split.
        /// </summary>
        public bool? IsSplit { get; set; }

        /// <summary>
        /// Indicates whether this closed post position has been edited.
        /// </summary>
        public bool? IsEdited { get; set; }

        /// <summary>
        /// Status of this closed post position history row.
        /// </summary>
        public ClosedPostPositionStatus? Status { get; set; }

        /// <summary>
        /// Indicates whether a questionnaire is required.
        /// </summary>
        public bool? QuestionaireRequired { get; set; }

        /// <summary>
        /// Foreign Key - Questionnaire response ID.
        /// </summary>
        public Guid? QuestionaireResponseID { get; set; }

        /// <summary>
        /// Parent closed post position id for edited/split history linkage.
        /// </summary>
        public Guid? ParentClosedPostPositionID { get; set; }


        public PostDto? Post { get; set; }
        public DateOnly? GetWorkDate()
        {
            return WorkStartDate ?? DateOfWork;
        }

        public TimeOnly? GetWorkTime()
        {
            return WorkStartTime ?? StartTime;
        }

        public int? GetPostLength()
        {
            if (!WorkStartTime.HasValue || !WorkEndTime.HasValue)
                return null;

            var date = WorkStartDate ?? new DateOnly(2000, 1, 1);

            var start = date.ToDateTime(WorkStartTime.Value);
            var end = date.ToDateTime(WorkEndTime.Value);

            var mins = (int)(end - start).TotalMinutes;

            return mins > 0 ? mins : null;
        }
    }
}
