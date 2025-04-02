namespace SecureP.Identity.Models.Result;

public class ValidationResult
{
    public bool Success { get; set; } = true;
    public string? Message { get; set; }
    public Dictionary<string, string>? Errors { get; set; }
}