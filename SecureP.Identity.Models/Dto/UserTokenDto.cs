namespace SecureP.Identity.Models.Dto;


public class GetUserTokenDto<TKey> where TKey : IEquatable<TKey>
{
    public string LoginProvider { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;
    public DateTime ExpiryDate { get; set; } = default!;
}