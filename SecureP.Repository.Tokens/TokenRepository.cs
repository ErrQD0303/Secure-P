using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SecureP.Identity.Models;
using SecureP.Repository.Abstraction;
using SecureP.Shared;
using SecureP.Shared.Configures;

namespace SecureP.Repository.Tokens;

public class TokenRepository<TKey> : ITokenRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly UserManager<AppUser<TKey>> _userManager;

    public TokenRepository(UserManager<AppUser<TKey>> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> AddTokenAsync(string token, TKey userId, TokenType tokenType, DateTime expiryDate, string loginProvider = AppConstants.DefaultLoginProvider)
    {
        var user = await _userManager.Users
            .Where(u => u.Id.Equals(userId))
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return false;
        }

        user.UserTokens ??= [];

        var existingToken = GetToken(user, tokenType, loginProvider);

        if (existingToken != null)
        {
            existingToken.Value = token;
            existingToken.ExpiryDate = expiryDate;
        }
        else
        {
            var userToken = new AppUserToken<TKey>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = tokenType.ToString(),
                Value = token,
                ExpiryDate = expiryDate
            };

            user.UserTokens.Add(userToken);
        }

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> RemoveTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider)
    {
        var user = await _userManager.Users
            .Where(u => u.Id.Equals(userId))
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return false;
        }

        user.UserTokens ??= [];

        var existingToken = GetToken(user, tokenType, loginProvider);

        if (existingToken is null)
        {
            return false;
        }

        user.UserTokens.Remove(existingToken);
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    public async Task<bool> ValidateTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var user = await _userManager.Users
            .Where(u => u.Id.Equals(userId))
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync();

        if (user is null)
        {
            return false;
        }

        user.UserTokens ??= [];

        var existingToken = GetToken(user, tokenType, loginProvider);

        if (existingToken is null || existingToken.Value != token)
        {
            return false;
        }

        if (existingToken.ExpiryDate < DateTime.UtcNow)
        {
            user.UserTokens.Remove(existingToken);
            await _userManager.UpdateAsync(user);

            return false;
        }

        return true;
    }

    private static AppUserToken<TKey>? GetToken(AppUser<TKey> user, TokenType tokenType, string loginProvider)
    {
        return user.UserTokens.FirstOrDefault(t =>
            t.LoginProvider == loginProvider &&
            t.Name == tokenType.ToString() &&
            t.UserId.Equals(user.Id));
    }
}
