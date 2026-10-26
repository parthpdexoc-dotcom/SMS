using SMS.Generic.Dtos.JsonConverters;
using SMS.Generic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SMS.Generic.Dtos.Dtos
{
    public class AvailablePostDto
    {
        public Guid RequestID { get; set; }
        public Guid PostID { get; set; }

        public DateOnly DateOfWork { get; set; }

        public string PostTitle { get; set; } = null!;
        public string? RequestTitle { get; set; } = null;
        public Guid? RequesterID { get; set; }
        public string? RequesterTitle { get; set; }
        public Guid? PayCodeID { get; set; }
        public string? PayCodeValue { get; set; } = null;
        public string? JobRank { get; set; } = null;
        public TimeOnly? PostStartTime { get; set; }
        public TimeOnly? PostEndTime { get; set; }
        public int? PostLength { get; set; }
        public decimal? PostPayRate { get; set; }
        public decimal? PostPremiumRate { get; set; }
        public string PostLocation { get; set; }
        public string? RequestLocation { get; set; }
        public int TotalPositions { get; set; }
        public int AvailablePositions { get; set; }
        public bool IsSpecialEvent { get; set; }
        public bool? DefineCallSign { get; set; }
    }
    public class AvailablePostResultDto
    {
        public List<AvailablePostDto>? Posts { get; set; } = new();
        public string? Message { get; set; }

        public bool HasError => !string.IsNullOrEmpty(Message);
        public bool HasOutstandingReadyToClosePost { get; set; }
        public bool HasTimeConflictRequiringConfirmation { get; set; }

        /// <summary>
        /// When true with <see cref="Message"/>, UI should show a modal/dialog (e.g. cancel-pending post but slots still available).
        /// </summary>
        public bool HasCancellationPendingSlotsRequiringDialog { get; set; }
        public bool HasNoSlot { get; set; }
    }

    public class EmployeeDashboardCountsDto
    {
        public int AvailablePosts { get; set; }
        public int ScheduledJobs { get; set; }
        public int ClosedJobs { get; set; }
        public int PendingJobs { get; set; }
    }

    public class SearchModel
    {
        public DateOnly StartDate { get; set; }
        public int NumberOfDays { get; set; }
        public ReleaseAs ReleaseAs { get; set; } = ReleaseAs.Open;
        public bool DisplayWaitingList { get; set; }
        public Guid? Request { get; set; }
        public Guid? Requester { get; set; }
        public Guid? PayCode { get; set; }
    }

    /// <summary>
    /// Minimal id/title pair for dashboard Request Title filter dropdown.
    /// </summary>
    public class RequestFilterOptionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    public class ApplyPostPositionRequest
    {
        public DateOnly DateOfWork { get; set; }
    }

}
