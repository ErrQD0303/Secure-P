namespace SecureP.Service.Abstraction.Entities;

public class LogoutResponse<TKey> where TKey : IEquatable<TKey>
{
    public int StatusCode { get; set; }
    public string Success { get; set; } = default!;
    public string Message { get; set; } = default!;
}