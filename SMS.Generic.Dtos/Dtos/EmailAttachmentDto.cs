namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Email file attachment payload.
    /// </summary>
    public class EmailAttachmentDto
    {
        public string FileName { get; set; } = string.Empty;

        public string? ContentType { get; set; }

        public byte[] Content { get; set; } = Array.Empty<byte>();

        public Guid? FileUploadedId { get; set; }
    }
}
