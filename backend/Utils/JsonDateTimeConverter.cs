using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hotel_MVP.Utils
{
    public class JsonDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Форматируем дату в стандарт ISO 8601, понятный для JavaScript
            writer.WriteStringValue(value.ToUniversalTime().ToString("o"));
        }
    }
}