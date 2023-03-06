using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets.Grants;

public interface ISnowflakeGrantAsset
{
    public string GetGrantStatement(List<Privilege> privileges);
    public string GetRevokeStatement(List<Privilege> privileges);
    protected bool ValidateGrants(List<Privilege> privileges);
}