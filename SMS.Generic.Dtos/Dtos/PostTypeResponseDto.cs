using Newtonsoft.Json;
using SMS.Generic.Dtos.JsonConverters;
using SMS.Generic.Enums;
using System;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    public class PostTypeResponseDto : BaseDto
    {
        // Post-level fields
        public string PostTitle { get; set; } = null!;
        public string PostLocation { get; set; } = string.Empty;
        public Guid? RequestID { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool PostRequiresVehicle { get; set; }
        public string? PostDescription { get; set; }
        public decimal PostPayRate { get; set; }
        public decimal PostPremiumRate { get; set; }
        public bool PostIsFlexible { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public bool PostRequiresEquipment { get; set; }
        public bool IsSkillsRequired { get; set; }
        public bool RankEligible { get; set; }
        public string? JobRank { get; set; }
        public string? RequestTitle { get; set; }
        public Guid? RequesterID { get; set; }
        /// <summary>Requester display name from the job-level <c>Requester.Name</c>.</summary>
        public string? RequesterTitle { get; set; }
        public string? PayCodeValue { get; set; }
        public bool IsPlaceholder { get; set; }
        public bool EnableInvoiceAndPayroll { get; set; }
        public bool IsPostRequiredDocument { get; set; }
        public int TotalPostPositions { get; set; }

        // Release mode (from parent Request.ReleaseAs)
        public ReleaseAs ReleaseAs { get; set; } = ReleaseAs.Open;

        [JsonProperty("postPositionId")]
        [JsonPropertyName("postPositionId")]
        public Guid? PostPositionID { get; set; }
        public Guid PostID { get; set; }
        public Guid EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public DateOnly? DateOfWork { get; set; }

        public TimeOnly? PostStartTime { get; set; }
        public TimeOnly? PostEndTime { get; set; }
        public int PostLength { get; set; }
        public bool EquipmentRequired { get; set; }
        public string? CallSign { get; set; }
        public PostPositionStatus PostPositionStatus { get; set; }
        /// <summary>Name of the employee (from <c>PostPosition.EmployeeID</c>) for cancelled or cancellation-pending positions.</summary>
        public string? CancelledBy { get; set; }
        public bool WasUnclosed { get; set; }
        public Guid? ParentPostPositionId { get; set; }
        public bool IsSplit { get; set; } = false;
        public Guid? SplitPostPositionID { get; set; }
        public bool ShowVehicle { get; set; }
        public bool ShowEquipment { get; set; }

        /// <summary>
        /// When true, vehicle/equipment visibility is locked to <c>PostPositionVehicle</c> rows only.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsAllocationFinalized { get; set; }

        [Obsolete("Use ViewDetailsEquipmentLines.ComputeShowFlagsForViewer via ApplyVehicleEquipmentVisibilityAsync.")]
        public void ConfigureVehicleAndEquipmentVisibility(
            bool isHighestRankForDay,
            bool requestRequiresVehicle = false,
            bool requestRequiresEquipment = false)
        {
            ShowVehicle = isHighestRankForDay && (PostRequiresVehicle || requestRequiresVehicle);
            ShowEquipment = isHighestRankForDay && (PostRequiresEquipment || requestRequiresEquipment);
        }
    }
}
