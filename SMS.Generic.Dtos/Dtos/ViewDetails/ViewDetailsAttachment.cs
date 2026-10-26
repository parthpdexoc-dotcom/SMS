namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Attachment row for View Details downloads list.</summary>
    public class ViewDetailsAttachment
    {
        public string FileUploadedId { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string TypeOfFile { get; set; } = null!;
        public string? RequestId { get; set; }
        public long? FileSize { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
