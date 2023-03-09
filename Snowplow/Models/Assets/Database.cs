using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class Database: ISnowflakeAsset
{
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
            default:
                throw new NotImplementedException("Ownership is not implementer for this interface type");
        }
        
        return string.Format(@"
CREATE OR REPLACE DATABASE {0} COMMENT = '{1}';
GRANT OWNERSHIP ON DATABASE {0} TO {2} {3};", 
            Name, Comment, ownerType.GetSnowflakeType(), Owner.GetIdentifier()
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