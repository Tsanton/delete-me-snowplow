using Snowflake.Client.Model;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets.Grants;

public class SchemaGrant<T> : ISnowflakeGrantAsset where T: ISnowflakeGrantable 
{
    public T Grantable { get; init; }
    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public string GetGrantStatement(List<Privilege> privileges)
    {
        var privs = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (Grantable)
        {
            case Role:
                return string.Format("GRANT {0} ON SCHEMA {1}.{2} TO ROLE {3};", 
                    privs, 
                    DatabaseName, 
                    SchemaName,
                    Grantable.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("GRANT {0} ON SCHEMA {1}.{2} TO DATABASE ROLE {3};", 
                    privs, 
                    DatabaseName, 
                    SchemaName,
                    Grantable.GetIdentifier()
                );
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
                return string.Format("REVOKE {0} ON SCHEMA {1}.{2} FROM ROLE {3} CASCADE;",
                    privs,
                    DatabaseName, 
                    SchemaName,
                    Grantable.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("REVOKE {0} ON SCHEMA {1}.{2} FROM DATABASE ROLE {3} CASCADE;", 
                    privs, 
                    DatabaseName, 
                    SchemaName,
                    Grantable.GetIdentifier()
                );
            default:
                throw new NotImplementedException("GetRevokeStatement is not implemented for this interface type");
        }
    }

    public bool ValidateGrants(List<Privilege> privileges)
    {
        throw new NotImplementedException();
    }
}