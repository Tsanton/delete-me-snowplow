using System.Text.Json.Serialization;

namespace Snowplow.Models.Entities;

public class Schema:ISnowflakeEntity
{
    public string Name { get; set; }
    [JsonPropertyName("database_name")]
    public string DatabaseName { get; set; }
    public string Owner { get; set; }
    public string Comment { get; set; }
    [JsonPropertyName("retention_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int RetentionTime { get; set; }
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; set; }
}