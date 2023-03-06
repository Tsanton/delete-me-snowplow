using Snowflake.Client.Model;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets.Grants;

public class DatabaseGrant<T> : ISnowflakeGrantAsset where T: ISnowflakeGrantable 
{
    public T Grantable { get; init; }
    public string DatabaseName { get; init; }
    public string GetGrantStatement(List<Privilege> privileges)
    {
        var privs = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (Grantable)
        {
            case Role:
                return string.Format("GRANT {0} ON DATABASE {1} TO ROLE {2};", privs, DatabaseName, Grantable.GetIdentifier());
            case DatabaseRole:
                return string.Format("GRANT {0} ON DATABASE {1} TO DATABASE ROLE {2};", privs, DatabaseName, Grantable.GetIdentifier());
            default:
                throw new NotImplementedException("GetGrantStatement is not implemented for this interface type");
        }
    }

    public string GetRevokeStatement(List<Privilege> privileges)
    {
        var privs = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (Grantable)
        {
            case Role:
                return string.Format("REVOKE {0} ON DATABASE {1} FROM ROLE {2} CASCADE;", privs, DatabaseName, Grantable.GetIdentifier());
            case DatabaseRole:
                return string.Format("REVOKE {0} ON DATABASE {1} FROM DATABASE ROLE {2} CASCADE;", privs, DatabaseName, Grantable.GetIdentifier());
            default:
                throw new NotImplementedException("GetRevokeStatement is not implemented for this interface type");
        }
    }

    public bool ValidateGrants(List<Privilege> privileges)
    {
        throw new NotImplementedException();
    }
}