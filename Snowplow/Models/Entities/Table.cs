using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;

public class Column
{
    public string Name { get; set; }
    [JsonPropertyName("type")]
    public string ColumnType { get; set; }
    public string? Default { get; set; }
    public string? Check { get; set; }
    public string? Expression { get; set; }
    [JsonPropertyName("null?")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool Nullable { get; set; }
    [JsonPropertyName("primary key")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool PrimaryKey { get; set; }
    [JsonPropertyName("unique key")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool UniqueKey { get; set; }
    [JsonPropertyName("policy name")]
    public string? PolicyName { get; set; }
    [JsonPropertyName("auto_increment")]
    public string? AutoIncrement { get; set; }
    public string Comment { get; set; }
}

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
