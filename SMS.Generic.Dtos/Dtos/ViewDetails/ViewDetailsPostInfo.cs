namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Post slice for View Details modal (notes / header use <see cref="PostId"/>).</summary>
    public class ViewDetailsPostInfo
    {
        public Guid PostId { get; set; }
        public string PostTitle { get; set; } = string.Empty;
        public string? PostDescription { get; set; }
        public string PostLocation { get; set; } = string.Empty;
        public TimeOnly PostStartTime { get; set; }
        public TimeOnly PostEndTime { get; set; }
        public int PostLength { get; set; }
        public int TotalPostPositions { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? PayCodeId { get; set; }
        public string? PayCodeValue { get; set; }
        public string? PayCodeDescription { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? PrimaryPhone { get; set; }
        public decimal PostPayRate { get; set; }
        public bool IsPremiumPaidEvent { get; set; }
        public bool PostIsFlexible { get; set; }
    }
}
