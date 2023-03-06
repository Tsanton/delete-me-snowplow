using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;

public class RoleInheritance: ISnowflakeEntity
{
    [JsonPropertyName("grantee_name")]
    public string PrincipalIdentifier { get; init; }
    [JsonPropertyName("granted_to")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("name")]
    public string InheritedRoleIdentifier { get; init; }
    [JsonPropertyName("granted_on")]
    public SnowflakePrincipal InheritedRoleType { get; init; }
    public Privilege Privilege { get; init; }
    [JsonPropertyName("grant_option")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool GrantOption { get; init; }
    [JsonPropertyName("granted_by")]
    public string GrantedBy { get; init; }
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
}