namespace Snowplow.Models.Describables;

public class Database: ISnowflakeDescribable
{
    public string Name { get; init; }
    
    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW DATABASES LIKE '{0}';",Name).ToUpper();
    }
}