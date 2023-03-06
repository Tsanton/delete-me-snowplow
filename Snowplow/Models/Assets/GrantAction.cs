using Snowplow.Models.Assets.Grants;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Assets;

public class GrantAction: ISnowflakeAsset
{
    public ISnowflakeGrantAsset Target { get; init; }
    public List<Privilege> Privileges { get; set; }
    
    public string GetCreateStatement()
    {
        return Target.GetGrantStatement(Privileges);
    }

    public string GetDeleteStatement()
    {
        return Target.GetRevokeStatement(Privileges);
    }
}
