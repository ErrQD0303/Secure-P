using Microsoft.AspNetCore.Http;
using SecureP.Service.Abstraction;
using SecureP.Shared;

namespace SecureP.Service.UploadService;

public class UploadService<TKey> : IUploadService<TKey> where TKey : IEquatable<TKey>
{
    private readonly IUserService<TKey> _userService;

    public UploadService(IUserService<TKey> userService)
    {
        _userService = userService;
    }

    public async Task<string?> UploadAvatarAsync(IFormFile formFile, TKey userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var contentRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var userAvatarDirectory = Path.Combine(contentRootPath, AppConstants.UserAvatarFolder);
        if (!Directory.Exists(userAvatarDirectory))
        {
            Directory.CreateDirectory(userAvatarDirectory);
        }

        var avatar = formFile.FileName;
        var avatarExtension = Path.GetExtension(avatar);

        user.Avatar = Path.Combine("", AppConstants.UserAvatarFolder, $"{userId}{avatarExtension}").Replace('\\', '/');
        await _userService.UpdateUserAsync(user);

        var userAvatarPath = Path.Combine(contentRootPath, user.Avatar);
        System.Console.WriteLine(userAvatarPath);
        using var stream = new FileStream(userAvatarPath, FileMode.Create);
        await formFile.CopyToAsync(stream);

        return user.Avatar;
    }
}
