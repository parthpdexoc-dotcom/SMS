namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Post equipment with resolved department equipment type display name for API listing.
    /// </summary>
    public sealed class PostEquipmentWithNameDto : PostEquipmentDto
    {
        /// <summary>
        /// Display name from equipment type (short title preferred; otherwise description).
        /// </summary>
        public string? EquipmentName { get; set; }
    }
}
