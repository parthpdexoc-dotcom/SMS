namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Calendar invite metadata used to generate an ICS attachment.
    /// </summary>
    public class EmailCalendarInviteDto
    {
        public string Summary { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Uid { get; set; } = Guid.NewGuid().ToString("N");
        public string FileName { get; set; } = "invite.ics";
    }
}
