using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class Schema:ISnowflakeAsset
{

    public Database Database { get; init; }
    public string Name { get; init; }
    public string Comment { get; init; }
    public ISnowflakeGrantable Owner { get; init; } = new Role{Name = "SYSADMIN"};
    public string GetCreateStatement()
    {
        SnowflakePrincipal ownerType;
        switch (Owner)
        {
            case Role principal:
                ownerType = SnowflakePrincipal.Role;
                break;
            case DatabaseRole principal:
                ownerType = SnowflakePrincipal.DatabaseRole;
                break;
            default:
                throw new NotImplementedException("Ownership is not implementer for this interface type");
        }
        return string.Format(@"
CREATE OR REPLACE SCHEMA {0}.{1} WITH MANAGED ACCESS COMMENT = '{2}';
GRANT OWNERSHIP ON SCHEMA {0}.{1} TO {3} {4} REVOKE CURRENT GRANTS;", 
            Database.Name, Name, Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
        );
    }

    public string GetDeleteStatement()
    {
        return string.Format(@"DROP SCHEMA IF EXISTS {0}.{1} CASCADE;", 
            Database.Name, Name
        );
    }
}