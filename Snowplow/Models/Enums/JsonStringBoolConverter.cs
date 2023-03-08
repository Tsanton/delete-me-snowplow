using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snowplow.Models.Enums;

public class JsonStringBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var input = reader.GetString();
        return input!.ToUpper() switch
        {
            "Y" => true,
            "N" => false,
            _ => bool.Parse(input)
        };
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}