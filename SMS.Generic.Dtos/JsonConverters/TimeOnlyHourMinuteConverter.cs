namespace SMS.Generic.Dtos.JsonConverters
{
    /// <summary>
    /// Newtonsoft.Json converter for <see cref="TimeOnly"/> (e.g. "15:00", "09:30").
    /// Handles TimeOnly for all JSON when Newtonsoft is used.
    /// </summary>
    public class NewtonsoftTimeOnlyConverter : Newtonsoft.Json.JsonConverter<TimeOnly>
    {
        private const string Format = "HH:mm";

        public override TimeOnly ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Null || reader.Value == null)
                return default;
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Date && reader.Value is DateTime dt)
                return TimeOnly.FromDateTime(dt);
            if (reader.TokenType == Newtonsoft.Json.JsonToken.String)
            {
                var s = reader.Value?.ToString();
                return string.IsNullOrEmpty(s) ? default : TimeOnly.Parse(s!);
            }
            throw new Newtonsoft.Json.JsonSerializationException($"Unexpected token {reader.TokenType} when parsing TimeOnly.");
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, TimeOnly value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(Format));
        }
    }

    /// <summary>
    /// Newtonsoft.Json converter for <see cref="TimeOnly?"/> (e.g. "15:00" or null).
    /// Used when Newtonsoft is selected for request/response (e.g. JSON Patch).
    /// </summary>
    public class NewtonsoftNullableTimeOnlyConverter : Newtonsoft.Json.JsonConverter<TimeOnly?>
    {
        private const string Format = "HH:mm";

        public override TimeOnly? ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, TimeOnly? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Null || reader.Value == null)
                return null;
            if (reader.TokenType == Newtonsoft.Json.JsonToken.Date && reader.Value is DateTime dt)
                return TimeOnly.FromDateTime(dt);
            if (reader.TokenType == Newtonsoft.Json.JsonToken.String)
            {
                var s = reader.Value?.ToString();
                return string.IsNullOrEmpty(s) ? null : TimeOnly.Parse(s);
            }
            throw new Newtonsoft.Json.JsonSerializationException($"Unexpected token {reader.TokenType} when parsing TimeOnly?.");
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, TimeOnly? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
                writer.WriteValue(value.Value.ToString(Format));
        }
    }
}
