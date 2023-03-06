using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;

public class PrincipalAscendant
{
    [JsonPropertyName("grantee_name")]
    public string GranteeIdentifier { get; init; }
    [JsonPropertyName("granted_to")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("role")]
    public string GrantedIdentifier { get; init; }
    [JsonPropertyName("granted_on")]
    public SnowflakePrincipal GrantedOn { get; init; }
    [JsonPropertyName("granted_by")]
    public string GrantedBy { get; init; }
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
    [JsonPropertyName("distance_from_source")]
    public int DistanceFromSource { get; init; } = 0;
}


public class PrincipalAscendants:ISnowflakeEntity
{
    [JsonPropertyName("principal_identifier")]
    public string PrincipalIdentifier { get; init; }
    [JsonPropertyName("principal_type")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("ascendants")]
    public List<PrincipalAscendant> Ascendants { get; init; }
}
