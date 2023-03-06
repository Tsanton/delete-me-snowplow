namespace Snowplow.Models.Describables;

public interface ISnowflakeDescribable
{
    public string GetDescribeStatement();
}
