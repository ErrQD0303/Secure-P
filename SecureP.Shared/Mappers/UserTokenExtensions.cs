using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;

namespace SecureP.Shared.Mappers;

public static class UserTokenExtensions
{
    public static GetUserTokenDto<TKey> ToGetUserTokenDto<TKey>(this AppUserToken<TKey> userToken) where TKey : IEquatable<TKey>
    {
        return new GetUserTokenDto<TKey>
        {
            LoginProvider = userToken.LoginProvider,
            Name = userToken.Name,
            Value = userToken.Value!,
            ExpiryDate = userToken.ExpiryDate
        };
    }
}