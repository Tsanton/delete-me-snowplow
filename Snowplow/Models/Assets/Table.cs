using Microsoft.VisualBasic;

namespace Snowplow.Models.Assets;

public class Table:ISnowflakeAsset
{
    public string? DatabaseName { get; init; }
    public string? SchemaName { get; init; }
    public string? TableName { get; init; }
    // public string? TableType { get; init; } //[ { [ LOCAL | GLOBAL ] TEMPORARY | VOLATILE } | TRANSIENT ]
    public Queue<ISnowflakeColumn>? Columns { get; init; }
    // public List<Tag> Tags { get; init; }
    // public RowAccessPolicy RowAccessPolicy { get; init; }
    public int DataRetentionTimeInDays { get; init; } = 1;
    public string Comment { get; init; } = "";
    
    public string GetCreateStatement()
    {
        var columns = $"(\n{string.Join(",\n", Columns!.Select(x => x.GetDefinition()))}\n)";
        return $"CREATE TABLE {DatabaseName}.{SchemaName}.{TableName} {columns}";
    }

    public string GetDeleteStatement()
    {
        return $"DROP TABLE {DatabaseName}.{SchemaName}.{TableName}";
    }
}