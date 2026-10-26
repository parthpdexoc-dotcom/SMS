using Newtonsoft.Json;
using SMS.Generic.Enums;
using System.Text.Json.Serialization;

namespace SMS.Generic.Dtos.Dtos
{
    /// <summary>
    /// Email type as a selectable option (numeric value and display label).
    /// </summary>
    public class EmailTypeOptionDto
    {
        /// <summary>
        /// Underlying <see cref="EmailType"/> value.
        /// </summary>
        public int Value { get; init; }

        /// <summary>
        /// Human-readable label for the email type.
        /// </summary>
        public string Label { get; init; } = string.Empty;
    }
}
