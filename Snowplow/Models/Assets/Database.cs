namespace Snowplow.Models.Assets;

public class Database: ISnowflakeAsset
{
    public string Name { get; init; }
    public string Comment { get; init; }
    public string Owner { get; init; }
    
    public string GetCreateStatement()
    {
        return string.Format(@"
CREATE OR REPLACE DATABASE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON DATABASE {0} TO {2};", 
            Name, Comment, Owner
        );
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format(@"
DROP DATABASE IF EXISTS {0} CASCADE;
", Name);
    }
}