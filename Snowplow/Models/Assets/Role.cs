namespace Snowplow.Models.Assets;

public class Role: ISnowflakeAsset, ISnowflakeGrantable
{
    public string Name { get; init; }
    public string Comment { get; init; }
    public string Owner { get; init; } = "USERADMIN";
    public string GetCreateStatement()
    {
        return string.Format(@"
CREATE OR REPLACE ROLE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON ROLE {0} TO {2} REVOKE CURRENT GRANTS;", 
            GetIdentifier(), Comment, Owner
        );
    }

    public string GetDeleteStatement()
    {
        // ReSharper disable once UseStringInterpolation
        return string.Format("DROP ROLE IF EXISTS {0};", GetIdentifier());
    }

    public string GetIdentifier()
    {
        return Name;
    }
}