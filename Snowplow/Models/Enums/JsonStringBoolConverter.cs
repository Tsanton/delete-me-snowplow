using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowplow.Models.Enums;

public class JsonStringBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return bool.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}