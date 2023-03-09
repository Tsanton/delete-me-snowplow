using System.Text;
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
        var sb = new StringBuilder();
        var columnDefinitions = new List<string>();
        var primaryKeys = new List<string>();
        sb.AppendLine($"CREATE TABLE {DatabaseName}.{SchemaName}.{TableName} (");
        foreach (var c in Columns!)
        {
            columnDefinitions.Add(c.GetDefinition());
            if (c.PrimaryKey) primaryKeys.Add(c.Name);
        }
        if (primaryKeys.Count > 0) sb.Append(string.Join(",\n", columnDefinitions)); else sb.AppendLine(string.Join(",\n", columnDefinitions));
        if (primaryKeys.Count > 0) sb.AppendLine(",").AppendLine($"PRIMARY KEY({string.Join(", ", primaryKeys)})");
        sb.Append(')');  
        var test = sb.ToString();
        return test;
    }

    public string GetDeleteStatement()
    {
        return $"DROP TABLE {DatabaseName}.{SchemaName}.{TableName}";
    }
}