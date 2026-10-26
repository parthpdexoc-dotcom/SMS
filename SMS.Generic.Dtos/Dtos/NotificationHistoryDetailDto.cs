namespace SMS.Generic.Dtos.Dtos
{
    public class NotificationHistoryDetailDto : NotificationHistoryListItemDto
    {
        public string Body { get; set; } = string.Empty;

        public List<NotificationHistoryRecipientDto> To { get; set; } = new();

        public List<NotificationHistoryRecipientDto> Cc { get; set; } = new();

        public List<NotificationHistoryRecipientDto> Bcc { get; set; } = new();

        public List<NotificationHistoryAttachmentDto> Attachments { get; set; } = new();
    }
}
