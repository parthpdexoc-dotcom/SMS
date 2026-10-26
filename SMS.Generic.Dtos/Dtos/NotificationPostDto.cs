namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// DTO representing a post for notification.
    /// </summary>
    public class NotificationPostDto
    {
        public Guid PostId { get; set; }

        public string? PostTitle { get; set; }

        public string? PostLocation { get; set; }
    }
}
