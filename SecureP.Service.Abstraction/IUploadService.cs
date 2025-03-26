using Microsoft.AspNetCore.Http;

namespace SecureP.Service.Abstraction;

public interface IUploadService<TKey> where TKey : IEquatable<TKey>
{
    Task<string?> UploadAvatarAsync(IFormFile formFile, TKey userId);
}