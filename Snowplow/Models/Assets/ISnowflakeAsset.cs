namespace Snowplow.Models.Assets;

public interface ISnowflakeAsset
{
    public string GetCreateStatement();
    public string GetDeleteStatement();
}