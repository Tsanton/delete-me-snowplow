using System.Text.Json.Serialization;

namespace Snowplow.Models.Entities;

public class Database: ISnowflakeEntity
{
    public string Name { get; set; }
    public string Owner { get; set; }
    public string Origin { get; set; }
    public string Comment { get; set; }
    [JsonPropertyName("retention_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int RetentionTime { get; set; }
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }
}