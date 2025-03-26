using Microsoft.AspNetCore.Mvc;

namespace SecureP.Service.Abstraction.Entities;

public class ConfirmEmailRequest
{
    [FromQuery(Name = "email")]
    public required string Email { get; set; }
    [FromQuery(Name = "token")]
    public required string Token { get; set; }
}