namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Common request body for lightweight dropdown option endpoints.
    /// </summary>
    public class SelectOptionsRequestDto
    {
        public string? SearchTerm { get; set; }

        public Guid? SelectedId { get; set; }

        public List<Guid>? SelectedIds { get; set; }

        public Guid? RequesterId { get; set; }

        /// <summary>
        /// Returns the request body or an empty instance when the client sends no body.
        /// </summary>
        public static SelectOptionsRequestDto OrDefault(SelectOptionsRequestDto? body)
            => body ?? new SelectOptionsRequestDto();
    }
}
