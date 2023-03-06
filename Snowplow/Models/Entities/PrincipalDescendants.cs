using System.Text.Json.Serialization;
using Snowplow.Models.Enums;

namespace Snowplow.Models.Entities;

public class PrincipalDescendants:ISnowflakeEntity
{
    [JsonPropertyName("principal_identifier")]
    public string PrincipalIdentifier { get; init; }
    [JsonPropertyName("principal_type")]
    public SnowflakePrincipal PrincipalType { get; init; }
    [JsonPropertyName("descendant_roles")]
    public List<Grant> DescendantRoles { get; init; }
}
