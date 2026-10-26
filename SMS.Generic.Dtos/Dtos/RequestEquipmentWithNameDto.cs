namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Request equipment with resolved department equipment type display name for API listing.
    /// </summary>
    public sealed class RequestEquipmentWithNameDto : RequestEquipmentDto
    {
        /// <summary>
        /// Display name from equipment type (short title preferred; otherwise description).
        /// </summary>
        public string? EquipmentName { get; set; }
    }
}
