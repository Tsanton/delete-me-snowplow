namespace Snowplow.Models.Describables;

public class Role: ISnowflakeDescribable, ISnowflakeGrantTarget
{
    public string Name { get; init; }

    public string GetDescribeStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("SHOW ROLES LIKE '{0}';",Name).ToUpper();
    }
}