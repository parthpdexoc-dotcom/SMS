using System.Text.Json.Serialization;

namespace SMS.Generic.Common.Extensions;

public abstract class BaseError
{
    /// <summary>
    /// specific code of error
    /// </summary>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// message of error
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
