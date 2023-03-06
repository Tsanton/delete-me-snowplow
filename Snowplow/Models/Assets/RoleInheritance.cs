using System.Data;
using Snowplow.Models.Assets.Grants;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class RoleInheritance: ISnowflakeAsset
{
    public ISnowflakeGrantable ChildRole { get; init; }
    public ISnowflakeGrantable ParentPrincipal { get; init; }
    public string GetCreateStatement()
    {
        SnowflakePrincipal childRoleType;
        SnowflakePrincipal parentPrincipalType;
        var grantStatement = "GRANT";
        switch (ChildRole)
        {
            case Role principal:
                childRoleType = SnowflakePrincipal.Role;
                grantStatement += $" ROLE {principal.GetIdentifier()} TO";
                break;
            case DatabaseRole principal:
                childRoleType = SnowflakePrincipal.DatabaseRole;
                grantStatement += $" DATABASE ROLE {principal.GetIdentifier()} TO";
                break;
            default:
                throw new NotImplementedException("GetIdentifier is not implemented for this interface type");
        }
        switch (ParentPrincipal)
        {
            case Role principal:
                parentPrincipalType = SnowflakePrincipal.Role;
                grantStatement += $" ROLE {principal.GetIdentifier()};";
                break;
            case DatabaseRole principal:
                parentPrincipalType = SnowflakePrincipal.DatabaseRole;
                grantStatement += $" DATABASE ROLE {principal.GetIdentifier()};";
                break;
            default:
                throw new NotImplementedException("GetIdentifier is not implemented for this interface type");
        }
        switch (parentPrincipalType)
        {
            case SnowflakePrincipal.DatabaseRole when childRoleType == SnowflakePrincipal.Role:
                throw new ConstraintException("Account roles cannot be granted to database roles");
            case SnowflakePrincipal.DatabaseRole when childRoleType == SnowflakePrincipal.DatabaseRole && ((DatabaseRole)ChildRole).DatabaseName != ((DatabaseRole)ParentPrincipal).DatabaseName:
                throw new ConstraintException("Can only grant database roles to other database roles in the same database");
        }
        return grantStatement;
    }

    public string GetDeleteStatement()
    {
        var revokeStatement = "REVOKE";
        switch (ChildRole)
        {
            case Role principal:
                revokeStatement += $" ROLE {principal.GetIdentifier()} FROM";
                break;
            case DatabaseRole principal:
                revokeStatement += $" DATABASE ROLE {principal.GetIdentifier()} FROM";
                break;
            default:
                throw new NotImplementedException("GetIdentifier is not implemented for this interface type");
        }
        switch (ParentPrincipal)
        {
            case Role principal:
                revokeStatement += $" ROLE {principal.GetIdentifier()};";
                break;
            case DatabaseRole principal:
                revokeStatement += $" DATABASE ROLE {principal.GetIdentifier()};";
                break;
            default:
                throw new NotImplementedException("GetIdentifier is not implemented for this interface type");
        }
        return revokeStatement;
    }
}