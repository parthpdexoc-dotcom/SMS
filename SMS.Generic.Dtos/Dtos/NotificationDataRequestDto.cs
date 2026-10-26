using SMS.Generic.Common.Implementations.Paginations;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request body for notification modal data.
    /// </summary>
    public class NotificationDataRequestDto
    {
        public Guid? PostPositionId { get; set; }

        public Guid? PostId { get; set; }

        public Guid? RequestId { get; set; }

        public List<Guid>? PostIds { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? SearchTerm { get; set; }

        public PageParam? PageParam { get; set; }
    }
}
