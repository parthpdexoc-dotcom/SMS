namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>Requester slice for View Details modal.</summary>
    public class ViewDetailsRequesterInfo
    {
        public Guid RequesterId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        /// <summary>Single formatted line from address parts when present.</summary>
        public string? Address { get; set; }
    }
}
