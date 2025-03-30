using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SecureP.Service.Abstraction.Entities;

public class UpdateProfileRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("phone_number")]
    public required string PhoneNumber { get; set; }
    [JsonPropertyName("day_of_birth")]
    public required DateTime DayOfBirth { get; set; }
}