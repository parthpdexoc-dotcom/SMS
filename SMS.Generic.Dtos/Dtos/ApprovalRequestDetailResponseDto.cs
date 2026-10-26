namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Response DTO for the approval detail view (by requestId and routingGroupId).
    /// Matches the structure of the Equipment/Vehicle Approval Request Received page.
    /// </summary>
    public class ApprovalRequestDetailResponseDto
    {
        /// <summary>Request ID.</summary>
        public Guid RequestId { get; set; }

        /// <summary>Routing group (approval group) ID.</summary>
        public Guid RoutingGroupId { get; set; }

        /// <summary>Requester information (name, contact, phone, email).</summary>
        public RequesterInfoDto RequesterInfo { get; set; }

        /// <summary>
        /// All approval routing groups for this request with their approval status.
        /// </summary>
        public List<ApprovalRoutingStatusDto> ApprovalRouting { get; set; }

        /// <summary>Request start date and vehicle summary (quantity, types).</summary>
        public RequestVehicleInfoDto RequestVehicleInfo { get; set; }

        /// <summary>Posts for this request. Empty array if none.</summary>
        public List<PostDetailDto> PostDetails { get; set; }

        /// <summary>
        /// When true, the current approver has already submitted a response for this request and routing group.
        /// The UI should show the "Vehicle Approval Recorded" confirmation screen instead of the approval form.
        /// </summary>
        public bool AlreadySubmittedByCurrentApprover { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalRequestDetailResponseDto"/> class.
        /// </summary>
        public ApprovalRequestDetailResponseDto(
            Guid requestId,
            Guid routingGroupId,
            RequesterInfoDto requesterInfo,
            RequestVehicleInfoDto requestVehicleInfo,
            List<PostDetailDto> postDetails,
            List<ApprovalRoutingStatusDto> approvalRouting)
        {
            RequestId = requestId;
            RoutingGroupId = routingGroupId;
            RequesterInfo = requesterInfo;
            RequestVehicleInfo = requestVehicleInfo;
            PostDetails = postDetails ?? new List<PostDetailDto>();
            ApprovalRouting = approvalRouting ?? new List<ApprovalRoutingStatusDto>();
        }
    }

    /// <summary>
    /// Approval routing entry with current approval and final approval flags.
    /// </summary>
    public class ApprovalRoutingStatusDto
    {
        /// <summary>
        /// Routing group (approval group) ID.
        /// </summary>
        public Guid RoutingGroupId { get; set; }

        /// <summary>
        /// Sequence order for this routing group in the approval flow.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Whether this routing group has approved the request.
        /// Null indicates no approval detail record yet.
        /// </summary>
        public bool? IsApproved { get; set; }

        /// <summary>
        /// Whether this approval entry represents the final approval for the request.
        /// Sourced from ApprovalDetail.FinalApproval.
        /// </summary>
        public bool? FinalApproval { get; set; }

        public ApprovalRoutingStatusDto(Guid routingGroupId, int rank, bool? isApproved, bool? finalApproval)
        {
            RoutingGroupId = routingGroupId;
            Rank = rank;
            IsApproved = isApproved;
            FinalApproval = finalApproval;
        }
    }

    /// <summary>
    /// Requester section: organization name, contact (name + phone), email.
    /// </summary>
    public class RequesterInfoDto
    {
        /// <summary>Requester/organization name.</summary>
        public string? Name { get; set; }

        /// <summary>Contact: requester contact name and phone combined (e.g. "John Smith - (555) 123-4567").</summary>
        public string? Contact { get; set; }

        /// <summary>Email address.</summary>
        public string? Email { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequesterInfoDto"/> class.
        /// </summary>
        public RequesterInfoDto(string? name, string? contact, string? email)
        {
            Name = name;
            Contact = contact;
            Email = email;
        }
    }

    /// <summary>
    /// Request details for vehicle: requested date/time, vehicles required count, vehicle type(s).
    /// </summary>
    public class RequestVehicleInfoDto
    {
        /// <summary>Requested date (request start date).</summary>
        public string? RequestedDate { get; set; }

        /// <summary>Number of vehicles required.</summary>
        public int VehiclesRequired { get; set; }

        /// <summary>Vehicle type name(s), e.g. "Marked Patrol Car". Comma-separated if multiple.</summary>
        public string? VehicleType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestVehicleInfoDto"/> class.
        /// </summary>
        public RequestVehicleInfoDto(string? requestedDate, int vehiclesRequired, string? vehicleType)
        {
            RequestedDate = requestedDate;
            VehiclesRequired = vehiclesRequired;
            VehicleType = vehicleType;
        }
    }

    /// <summary>
    /// Single post in the Post Details section.
    /// </summary>
    public class PostDetailDto
    {
        /// <summary>Post start time (formatted).</summary>
        public string? PostStartTime { get; set; }

        /// <summary>Post location.</summary>
        public string? Location { get; set; }

        /// <summary>Post title.</summary>
        public string? PostTitle { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostDetailDto"/> class.
        /// </summary>
        public PostDetailDto(string? postStartTime, string? location, string? postTitle)
        {
            PostStartTime = postStartTime;
            Location = location;
            PostTitle = postTitle;
        }
    }
}
