using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;
public class Table: ISnowflakeEntity
{   
    [JsonPropertyName("database_name")]
    public string DatabaseName { get; set; }
    [JsonPropertyName("schema_name")]
    public string SchemaName { get; set; }
    public string Name { get; set; }
    public string Kind { get; set; }
    public string Comment { get; set; }
    [JsonPropertyName("change_tracking")]
    public string ChangeTracking { get; set; }
    [JsonPropertyName("automatic_clustering")]
    public string AutomaticClustering { get; set; }
    public int Rows { get; set; }
    public string Owner { get; set; }
    [JsonPropertyName("retention_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int RetentionTime { get; set; }
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; set; }
    public List<Column> Columns { get; set; }
}
