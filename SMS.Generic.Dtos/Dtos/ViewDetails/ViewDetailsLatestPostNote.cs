namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Latest post note for View Details (same data as <c>GET api/PostNotes/{postId}/latest-notes</c> when present).</summary>
    public class ViewDetailsLatestPostNote
    {
        public Guid NoteId { get; set; }
        public Guid PostId { get; set; }
        public string? Notes { get; set; }
        public string? Comments { get; set; }
        public bool IsLatest { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
