using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets.Grants;

public class SchemaObjectFutureGrant<T> : ISnowflakeGrantAsset where T: ISnowflakeGrantable 
{
    public T Grantable { get; init; }
    public string DatabaseName { get; init; }
    public string SchemaName { get; init; }
    public SnowflakeObject GrantTarget { get; init; }
    public string GetGrantStatement(List<Privilege> privileges)
    {
        var privs = string.Join(", ", privileges.Select(x => x.GetEnumJsonAttributeValue()));
        switch (Grantable)
        {
            case Role:
                return string.Format("GRANT {0} ON FUTURE {1} IN SCHEMA {2}.{3} TO ROLE {4};", 
                    privs, 
                    GrantTarget.ToPluralString(), 
                    DatabaseName,
                    SchemaName,
                    Grantable.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("GRANT {0} ON FUTURE {1} IN SCHEMA {2}.{3}  TO DATABASE ROLE {4};", 
                    privs,
                    GrantTarget.ToPluralString(),
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
                return string.Format("REVOKE {0} ON FUTURE {1} IN SCHEMA {2}.{3} FROM ROLE {4};", 
                    privs, 
                    GrantTarget.ToPluralString(), 
                    DatabaseName,
                    SchemaName,
                    Grantable.GetIdentifier()
                );
            case DatabaseRole:
                return string.Format("REVOKE {0} ON FUTURE {1} IN SCHEMA {2}.{3} FROM DATABASE ROLE {4};", 
                    privs,
                    GrantTarget.ToPluralString(),
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