using System.Text;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class ForeignKey
{
    public ForeignKey(string databaseName, string schemaName, string tableName, string columnName)
    {
        DatabaseName = databaseName;
        SchemaName = schemaName;
        TableName = tableName;
        ColumnName = columnName;
    }
    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string TableName { get; init; }
    public string ColumnName { get; init; }
}

public class Identity
{
    public int StartNumber { get; init; } = 1;
    public int IncrementNumber { get; init; } = 1;

    public override string ToString()
    {
        return $"IDENTITY START {StartNumber} INCREMENT {IncrementNumber}";
    }
}

public class MaskingPolicy
{
    public string PolicyName{ get; init; }
}

public class Tag
{
    public string TagName{ get; init; }
    public string TagValue{ get; init; }
}

public class Sequence
{
    
}

//https://hevodata.com/learn/snowflake-data-types/
public interface ISnowflakeColumn
{
    string Name { get; init; }
    bool PrimaryKey { get; init; }
    bool Nullable { get; init; }
    bool Unique { get; init; }
    ForeignKey? ForeignKey  { get; init; }
    // MaskingPolicy MakingPolicy { get; init; }
    // List<Tag> Tags { get; init; }
    string GetDefinition();
}

public class Varchar: ISnowflakeColumn
{
    public Varchar(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public int Length { get; init; } = 16777216; //Max bytes in string 
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public string? DefaultValue { get; init; }
    public ForeignKey? ForeignKey { get; init; } = null;
    public string? Collation { get; init; } = null;
    
    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("VARCHAR").Append($"({Length})");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (Collation is not null) sb.Append(' ').Append($"COLLATE '{Collation}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Number: ISnowflakeColumn
{
    public Number(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public int Precision { get; init; } = 38; //Precision refers to `xxx` of xxx.yyy -> max 38 * `x` 
    public int Scale { get; init; } = 0; //Scale refers to `yyy` of xxx.yyy -> max 37 * `y`
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public decimal? DefaultValue { get; init; }
    public Identity? Identity { get; init; } = null;
    public Sequence? Sequence { get; init; } = null;
    public ForeignKey? ForeignKey  { get; init; } = null;
    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("NUMBER").Append($"({Precision},{Scale})");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT {DefaultValue}");
        if (Identity is not null) sb.Append(' ').Append($"IDENTITY({Identity.StartNumber},{Identity.IncrementNumber})");
        if (Sequence is not null) throw new NotImplementedException("Sequences are not supported as of now");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Bool: ISnowflakeColumn
{
    public Bool(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public bool? DefaultValue { get; init; }
    public ForeignKey? ForeignKey { get; init; } = null;

    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("BOOLEAN");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT {DefaultValue.Value}"); ;
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}


public class Date: ISnowflakeColumn
{
    public Date(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public string? DefaultValue { get; init; }
    public ForeignKey? ForeignKey { get; init; } = null;
    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append($"DATE");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'"); ;
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Time: ISnowflakeColumn
{
    public Time(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public int Precision { get; init; } = 0; //0 == seconds, 9 == nanoseconds //TODO: Validate 9 <= x <= 0
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public ForeignKey? ForeignKey { get; init; } = null;
    public string? DefaultValue { get; init; }

    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append($"TIME").Append($"({Precision})");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'"); ;
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Timestamp: ISnowflakeColumn
{
    public Timestamp(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public SnowflakeTimestamp TimestampType { get; init; } = SnowflakeTimestamp.LocalTimeZone;
    public int Precision { get; init; } = 0; //0 == seconds, 9 == nanoseconds //TODO: Validate 9 <= x <= 0
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public string? DefaultValue { get; init; } = null;
    public ForeignKey? ForeignKey { get; init; } = null;
    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append(TimestampType.GetEnumJsonAttributeValue()).Append($"({Precision})");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'"); ;
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}

public class Variant: ISnowflakeColumn
{
    public Variant(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
    public bool PrimaryKey { get; init; } = false;
    public bool Nullable { get; init; } = false;
    public bool Unique { get; init; } = false;
    public string? DefaultValue { get; init; } = null;
    public ForeignKey? ForeignKey { get; init; } = null;

    public string GetDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(Name).Append(' ').Append("VARIANT");
        if (PrimaryKey) sb.Append(' ').Append($"PRIMARY KEY");
        if (Nullable == false) sb.Append(' ').Append($"NOT NULL");
        if (Unique) sb.Append(' ').Append($"UNIQUE");
        if (DefaultValue is not null) sb.Append(' ').Append($"DEFAULT '{DefaultValue}'");
        if (ForeignKey is not null) throw new NotImplementedException("ForeignKeys are not supported as of now");
        return sb.ToString();
    }
}