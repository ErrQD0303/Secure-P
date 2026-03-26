namespace SecureP.Service.Abstraction.Results;

public record Error(string Code, string Description)
{
    public static Error Validation(string code, string description) => new(code, description);
    public static Error Conflict(string code, string description) => new(code, description);
}