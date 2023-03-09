using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class Role: ISnowflakeAsset, ISnowflakeGrantable
{
    public string Name { get; init; }
    public string Comment { get; init; }
    public ISnowflakeGrantable Owner { get; init; }
    public string GetCreateStatement()
    {
        SnowflakePrincipal ownerType;
        switch (Owner)
        {
            case Role principal:
                ownerType = SnowflakePrincipal.Role;
                break;
            default:
                throw new NotImplementedException("Ownership is not implementer for this interface type");
        }
        return string.Format(@"
CREATE OR REPLACE ROLE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON ROLE {0} TO {2} {3} REVOKE CURRENT GRANTS;", 
            GetIdentifier(), Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
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