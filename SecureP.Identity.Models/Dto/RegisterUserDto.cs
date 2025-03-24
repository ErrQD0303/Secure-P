using System.ComponentModel.DataAnnotations;

namespace SecureP.Identity.Models.Dto;

public class RegisterValidatedUserDto<TKey> where TKey : IEquatable<TKey>
{
    [Required]
    public string? UserName { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    [Phone]
    public string? PhoneNumber { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime DayOfBirth { get; set; }
    [Required]
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    [Required]
    public string? FullName { get; set; }
    public string? PostCode { get; set; }
    public ICollection<string> LicensePlates { get; set; } = [];
}
