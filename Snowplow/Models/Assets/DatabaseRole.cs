using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class DatabaseRole: ISnowflakeAsset, ISnowflakeGrantable
{
    public string Name { get; init; }
    public string DatabaseName { get; init; }
    public string Comment { get; init; } = "";
    public ISnowflakeGrantable Owner { get; init; } = new Role{Name = "USERADMIN"};
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
CREATE OR REPLACE DATABASE ROLE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON DATABASE ROLE {0} TO {2} {3} REVOKE CURRENT GRANTS;", 
            GetIdentifier(), Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
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