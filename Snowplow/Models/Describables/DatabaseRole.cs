namespace Snowplow.Models.Describables;

public class DatabaseRole:ISnowflakeDescribable, ISnowflakeGrantTarget
{
    public string Name { get; init; }
    public string DatabaseName { get; init; }

    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW DATABASE ROLES LIKE '{0}' IN DATABASE {1};", Name, DatabaseName).ToUpper();
    }
}
