using Snowflake.Client.Model;
using Snowplow.Models.Enums;

// ReSharper disable UseStringInterpolation

namespace Snowplow.Models.Assets.Grants;

public class AccountGrant<T> : ISnowflakeGrantAsset where T: ISnowflakeGrantable 
{
    public T Grantable { get; init; }

    public string GetGrantStatement(List<Privilege> privileges)
    {
        var privs = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (Grantable)
        {
            case Role:
                return string.Format("GRANT {0} ON ACCOUNT TO ROLE {1};", privs, Grantable.GetIdentifier());
            case DatabaseRole:
                throw new SnowflakeException("Account privileges cannot be granted to database roles");
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
                return string.Format("REVOKE {0} ON ACCOUNT FROM ROLE {1} CASCADE;", privs, Grantable.GetIdentifier());
            case DatabaseRole:
                throw new SnowflakeException("Account privileges cannot be neither granted to nor revoked from database roles");
            default:
                throw new NotImplementedException("GetRevokeStatement is not implemented for this interface type");
        }
    }

    public bool ValidateGrants(List<Privilege> privileges)
    {
        throw new NotImplementedException();
    }
}