namespace Snowplow.Models.Assets;

public class DatabaseRole: ISnowflakeAsset, ISnowflakeGrantable
{
    public string Name { get; init; }
    public string DatabaseName { get; init; }
    public string Comment { get; init; }
    public string Owner { get; init; } = "USERADMIN";
    public string GetCreateStatement()
    {
        return string.Format(@"
CREATE OR REPLACE DATABASE ROLE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON DATABASE ROLE {0} TO {2} REVOKE CURRENT GRANTS;", 
            GetIdentifier(), Comment, Owner
        );
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("DROP DATABASE ROLE IF EXISTS {0};", GetIdentifier());
    }

    public string GetIdentifier()
    {
        return $"{DatabaseName}.{Name}";
    }
}