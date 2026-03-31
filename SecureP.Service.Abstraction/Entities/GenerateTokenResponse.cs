namespace SecureP.Service.Abstraction.Entities;

public class GenerateTokenResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public Dictionary<string, object> Errors { get; set; } = default!;
    public TokenResponseDto Tokens { get; set; } = default!;
}

public class RefreshTokenResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public Dictionary<string, object> Errors { get; set; } = default!;
    public TokenResponseDto Tokens { get; set; } = default!;
}