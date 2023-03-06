using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;

public class Grant: ISnowflakeEntity
{
    [JsonPropertyName("grantee_name")]
    public string GranteeIdentifier { get; init; }
    [JsonPropertyName("granted_to")]
    public SnowflakePrincipal PrincipalType { get; init; } //TODO:Not correct for DATABASE_ROLE in PrincipalDescendants
    [JsonPropertyName("granted_on")]
    public string GrantedOn { get; init; } //TODO: Enum
    [JsonPropertyName("name")]
    public string GrantedIdentifier { get; init; }
    public Privilege Privilege { get; init; }
    [JsonPropertyName("grant_option")]
    [JsonConverter(typeof(JsonStringBoolConverter))]
    public bool GrantOption { get; init; }
    [JsonPropertyName("granted_by")]
    public string GrantedBy { get; init; }
    [JsonPropertyName("created_on")]
    [JsonConverter(typeof(JsonSnowflakeDatetimeConverter))]
    public DateTime CreatedOn { get; init; }
    [JsonPropertyName("distance_from_source")]
    public int DistanceFromSource { get; init; } = 0;
}
