using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Repository.Abstraction;
using SecureP.Shared;

namespace SecureP.Repository.Tokens;

public class TokenRepository<TKey>(UserManager<AppUser<TKey>> userManager, AppDbContext<TKey> dbContext) : ITokenRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly UserManager<AppUser<TKey>> _userManager = userManager;
    private readonly AppDbContext<TKey> _context = dbContext;

    public async Task<bool> AddTokenAsync(string token, AppUser<TKey> user, TokenType tokenType, DateTime expiryDate)
    {
        var existingToken = user.UserTokens.FirstOrDefault(t => t.Name == tokenType.ToString() && t.UserId.Equals(user.Id));

        if (existingToken != null)
        {
            existingToken.Value = token;
            existingToken.ExpiryDate = expiryDate;
        }
        else
        {
            var tokenName = tokenType.ToString();

            var userToken = new AppUserToken<TKey>
            {
                UserId = user.Id,
                LoginProvider = user.UserLogins?.FirstOrDefault()?.LoginProvider ?? AppConstants.DefaultLoginProvider,
                Name = tokenName,
                Value = token,
                ExpiryDate = expiryDate
            };

            _context.UserTokens.Add(userToken);
        }

        var rowsAffected = await _context.SaveChangesAsync();

        return rowsAffected > 0;
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

    public async Task<bool> ValidateTokenAsync(string token, AppUser<TKey> user, TokenType tokenType)
    {
        var existingToken = user.UserTokens
            .FirstOrDefault(t => t.Name == tokenType.ToString()
                && t.UserId.Equals(user.Id)
                && t.Value == token);

        if (existingToken is null)
        {
            return false;
        }

        if (existingToken.ExpiryDate < DateTime.UtcNow)
        {
            await RemoveUserTokenAsync(user, existingToken);
            return false;
        }

        await RemoveUserTokenAsync(user, existingToken);
        return true;
    }

    private static async Task RemoveUserTokenAsync(AppUser<TKey> user, AppUserToken<TKey> existingToken, UserManager<AppUser<TKey>> userManager)
    {
        user.UserTokens.Remove(existingToken);
        await userManager.UpdateAsync(user);
    }

    public async Task RemoveUserTokenAsync(AppUser<TKey> user, AppUserToken<TKey> existingToken)
    {
        user.UserTokens.Remove(existingToken);
        await _context.SaveChangesAsync();
    }

    private static AppUserToken<TKey>? GetToken(AppUser<TKey> user, TokenType tokenType, string loginProvider)
    {
        return user.UserTokens.FirstOrDefault(t =>
            t.LoginProvider == loginProvider &&
            t.Name == tokenType.ToString() &&
            t.UserId.Equals(user.Id));
    }

    public async Task<AppUser<TKey>?> GetUserByTokenAsync(string token, TokenType tokenType)
    {
        return await _userManager.Users.Where(u => u.UserTokens.Any(t => t.Name == tokenType.ToString() && t.Value == token)).FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveTokenAsync(TKey userId, TokenType tokenType)
    {
        var user = _userManager.Users
            .Where(u => u.Id.Equals(userId))
            .Include(u => u.UserTokens)
            .FirstOrDefault();

        if (user is null)
        {
            return false;
        }

        user.UserTokens ??= [];

        var existingToken = user.UserTokens.FirstOrDefault(t => t.Name == tokenType.ToString() && t.UserId.Equals(userId));

        if (existingToken is null)
        {
            return false;
        }

        await RemoveUserTokenAsync(user, existingToken);
        return true;
    }
}
