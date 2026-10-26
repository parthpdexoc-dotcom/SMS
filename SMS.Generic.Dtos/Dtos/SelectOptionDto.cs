namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Common id/label pair for lightweight dropdown options.
    /// </summary>
    public class SelectOptionDto
    {
        [Newtonsoft.Json.JsonProperty("id")]
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public Guid Id { get; set; }

        [Newtonsoft.Json.JsonProperty("label")]
        [System.Text.Json.Serialization.JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Department equipment/vehicle dropdown option with default flag.
    /// </summary>
    public class DepartmentEquipmentSelectOptionDto : SelectOptionDto
    {
        [Newtonsoft.Json.JsonProperty("isDefault")]
        [System.Text.Json.Serialization.JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }
}
