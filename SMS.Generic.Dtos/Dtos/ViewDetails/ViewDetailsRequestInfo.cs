namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Request slice for View Details modal.</summary>
    public class ViewDetailsRequestInfo
    {
        public Guid RequestId { get; set; }
        public Guid? RequesterId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? Title { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Location { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public bool? PostLocationsAreStationary { get; set; }
    }
}
