using SecureP.Identity.Models.Dto;

namespace SecureP.Service.Abstraction.Entities;

public class GetAllParkingLocationResponse<TKey> where TKey : IEquatable<TKey>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public GetAllParkingLocationsDto<TKey>? Data { get; set; } = default!;
    public Dictionary<string, string> Errors { get; set; } = default!;
}