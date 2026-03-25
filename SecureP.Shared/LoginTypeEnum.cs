using System.Text.Json.Serialization;

namespace SecureP.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LoginType
{
    [JsonStringEnumMemberName("email")]
    Email,
    [JsonStringEnumMemberName("username")]
    Username,
    [JsonStringEnumMemberName("phone")]
    Phone
}