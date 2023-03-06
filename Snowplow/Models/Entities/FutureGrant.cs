using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;

public class FutureGrant: ISnowflakeEntity
{
    [JsonPropertyName("grantee_name")]
    public string RoleName { get; init; }
    [JsonPropertyName("grant_to")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("grant_on")]
    public string GrantOn { get; init; } //TODO: Enum
    [JsonPropertyName("name")]
    public string GrantTargetName { get; init; }
    public Privilege Privilege { get; init; }
    [JsonPropertyName("grant_option")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool GrantOption { get; init; }
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
}
