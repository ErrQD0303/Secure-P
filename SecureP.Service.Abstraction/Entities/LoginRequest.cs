using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class LoginRequestDto
{
    [EmailAddress]
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [Phone]
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    [DataType(DataType.Password)]
    [JsonPropertyName("password")]
    public string Password { get; set; } = default!;
}

public class LoginRequest
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}

public class LoginByEmailRequest : LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

public class LoginByUsernameRequest : LoginRequest
{
    [Required]
    public string Username { get; set; } = null!;
}
public class LoginByPhoneNumberRequest : LoginRequest
{
    [Required]
    public string Phone { get; set; } = null!;
}