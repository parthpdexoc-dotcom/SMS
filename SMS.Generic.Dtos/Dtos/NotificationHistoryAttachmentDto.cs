namespace SMS.Generic.Dtos.Dtos
{
    public class NotificationHistoryAttachmentDto
    {
        public Guid? FileUploadedId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public long? FileSizeBytes { get; set; }
    }
}
