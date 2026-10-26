using System.ComponentModel.DataAnnotations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request DTO for sending Vehicle Approval emails to approval groups.
    /// </summary>
    public class SendApprovalEmailsRequestDto
    {
        public SendApprovalEmailsRequestDto()
        {
        }

        public SendApprovalEmailsRequestDto(
            Guid requestId,
            Guid routingGroupId,
            string? systemUrl)
        {
            RequestId = requestId;
            RoutingGroupId = routingGroupId;
            SystemUrl = systemUrl;
        }

        /// <summary>
        /// The request ID (used to get request's FirstName, LastName, PrimaryEmail and to build the link).
        /// </summary>
        [Required]
        public Guid RequestId { get; set; }

        /// <summary>
        /// Approval group ID (RoutingGroup ApproverID). Used to fetch the group when ApproverEmail is not provided.
        /// </summary>
        public Guid RoutingGroupId { get; set; }

        /// <summary>
        /// The Vehicle Approval link (full URL) to use in the email. Provided by the caller; not built by the backend.
        /// </summary>
        public string? SystemUrl { get; set; }

    }
}
