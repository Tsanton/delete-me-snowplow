using System.Text.Json.Serialization;

namespace Snowplow.Models.Entities;

public class Role: ISnowflakeEntity
{
    public string Name { get; init; }
    public string Owner { get; init; }
    [JsonPropertyName("assigned_to_users")]
    public int AssignedToUsers { get; init; }
    [JsonPropertyName("granted_to_roles")]
    public int GrantedToRoles { get; init; }
    [JsonPropertyName("granted_roles")]
    public int GrantedRoles { get; init; }
    public string Comment { get; init; }
    [JsonPropertyName("created_on")]
    public DateTime CreatedOn { get; init; }
}