namespace Snowplow.Models.Assets;

public class Schema:ISnowflakeAsset
{

    public Database Database { get; init; }
    public string Name { get; init; }
    public string Comment { get; init; }
    public string Owner { get; init; } = "SYSADMIN";
    public string GetCreateStatement()
    {
        return string.Format(@"
CREATE OR REPLACE SCHEMA {0}.{1} WITH MANAGED ACCESS COMMENT = '{2}';
GRANT OWNERSHIP ON SCHEMA {0}.{1} TO {3} REVOKE CURRENT GRANTS;", 
            Database.Name, Name, Comment, Owner
        );
    }

    public string GetDeleteStatement()
    {
        return string.Format(@"DROP SCHEMA IF EXISTS {0}.{1} CASCADE;", 
            Database.Name, Name
        );
    }
}