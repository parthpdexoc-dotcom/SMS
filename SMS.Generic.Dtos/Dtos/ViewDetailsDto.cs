namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// <c>GET /api/Request/view-details</c> — minimal payload for SMSPostInformationModal only (camelCase JSON).
    /// </summary>
    public class ViewDetailsDto
    {
        public bool? IsSpecialEvent { get; set; }

        private int? _availablePositions;
        private int? _totalPostPositions;

        /// <summary>Remaining slots for the scoped context (single post + date, post-position branch, or special-event pool). Negative values are stored as <c>0</c>.</summary>
        public int? AvailablePositions
        {
            get => _availablePositions;
            set => _availablePositions = value is int n && n < 0 ? 0 : value;
        }

        /// <summary>Total post positions capacity for the same scope (for UI e.g. &quot;3 out of 5&quot;). Negative values are stored as <c>0</c>.</summary>
        public int? TotalPostPositions
        {
            get => _totalPostPositions;
            set => _totalPostPositions = value is int n && n < 0 ? 0 : value;
        }
        /// <summary>
        /// When <see cref="IsSpecialEvent"/> is true, pre-built Job Details text for the modal (includes the dynamic count). Otherwise null.
        /// </summary>
        public string? SpecialEventJobDetailsMessage { get; set; }
        public ViewDetailsPostInfo? Post { get; set; }
        public ViewDetailsPostPositionInfo? PostPosition { get; set; }
        public bool ShowVehicle { get; set; }
        public bool ShowEquipment { get; set; }
        public ViewDetailsRequestInfo? Request { get; set; }
        public ViewDetailsRequesterInfo? Requester { get; set; }
        public IReadOnlyList<ViewDetailsAttachment> Files { get; set; } = Array.Empty<ViewDetailsAttachment>();
                public IReadOnlyList<ViewDetailsEquipmentLine> Equipments { get; set; } = Array.Empty<ViewDetailsEquipmentLine>();
        /// <summary>Latest note for the resolved post (when <see cref="ViewDetailsPostInfo"/> exists); otherwise null.</summary>
        public ViewDetailsLatestPostNote? LatestPostNote { get; set; }

        /// <summary>
        /// Available-job / waiting-list: set <see cref="ShowVehicle"/> and <see cref="ShowEquipment"/> from post and request flags (no roster rank).
        /// </summary>
        public void ApplyVehicleEquipmentFlagsForAvailableJob(PostDto? fullPost, RequestDto? fullRequest)
        {
            if (fullPost != null)
            {
                ShowVehicle = fullPost.PostRequiresVehicle;
                ShowEquipment = fullPost.PostRequiresEquipment;
            }
            else
            {
                ShowVehicle = false;
                ShowEquipment = false;
            }
        }

        /// <summary>Clears post equipment lines (e.g. when nothing applies or before repopulating).</summary>
        public void ClearEquipmentLists()
        {
            Equipments = Array.Empty<ViewDetailsEquipmentLine>();
        }

        /// <summary>Available-job path: slot counts from <paramref name="fullPost"/> and <paramref name="dateOfWork"/>.</summary>
        public void ApplyAvailablePositionsForAvailablePost(PostDto? fullPost, DateOnly? dateOfWork)
        {
            if (!dateOfWork.HasValue || fullPost?.PostID is not { } pid || pid == Guid.Empty)
                return;

            TotalPostPositions = fullPost.TotalPostPositions;
            AvailablePositions = fullPost.GetAvailableSlotsForDate(dateOfWork.Value);
        }

        /// <summary>Post-position path: slot counts from assignment (optional <paramref name="dateOfWork"/> overrides work date).</summary>
        public void ApplyAvailablePositionsFromPostPosition(PostPositionDto postPosition, DateOnly? dateOfWork)
        {
            var slotDate = dateOfWork ?? postPosition.DateOfWork;
            if (!slotDate.HasValue || postPosition.Post == null)
                return;

            TotalPostPositions = postPosition.Post.TotalPostPositions;
            AvailablePositions = postPosition.Post.GetAvailableSlotsForDate(slotDate.Value);
        }
    }
}
