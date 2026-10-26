namespace SMS.Generic.Dtos.Dtos
{
    public class NotificationHistoryListItemDto
    {
        public Guid NotificationHistoryId { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string? SentBy { get; set; }

        public DateTime SentAt { get; set; }

        public int ToCount { get; set; }

        public int CcCount { get; set; }

        public int BccCount { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
