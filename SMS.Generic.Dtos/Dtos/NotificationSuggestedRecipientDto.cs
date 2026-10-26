namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Suggested contact email grouped by address with contributing sources.
    /// </summary>
    public class NotificationSuggestedRecipientDto
    {
        public string? DisplayName { get; set; }

        public string? PrimaryEmail { get; set; }

        public List<string> Sources { get; set; } = new();
    }
}
