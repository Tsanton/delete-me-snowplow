namespace Snowplow.Models.Describables;

public class Schema:ISnowflakeDescribable
{
    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW SCHEMAS LIKE '{0}' IN DATABASE {1};", SchemaName, DatabaseName).ToUpper();
    }
}