using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Repository.Abstraction;
using SecureP.Shared;

namespace SecureP.Repository.Tokens;

public class TokenRepository<TKey>(AppDbContext<TKey> dbContext) : ITokenRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly AppDbContext<TKey> _context = dbContext;

    public async Task AddTokenAsync(string token, AppUser<TKey> user, TokenType tokenType, DateTime expiryDate)
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
    }

    public async Task<bool> RemoveTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider)
    {
        var user = await _context.Users
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
        var result = await _context.SaveChangesAsync();

        return result > 0;
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

        // For OTPs, we want to invalidate the token immediately after validation to ensure it's single-use
        if (tokenType.Equals(TokenType.OTP))
        {
            await RemoveUserTokenAsync(user, existingToken);
        }

        return true;
    }

    public async Task RemoveUserTokenAsync(AppUser<TKey> user, AppUserToken<TKey> existingToken)
    {
        user.UserTokens.Remove(existingToken);
    }

    private static AppUserToken<TKey>? GetToken(AppUser<TKey> user, TokenType tokenType, string loginProvider)
    {
        return user.UserTokens.FirstOrDefault(t =>
            t.LoginProvider == loginProvider &&
            t.Name == tokenType.ToString() &&
            t.UserId.Equals(user.Id));
    }

    public Task<AppUser<TKey>?> GetUserByTokenAsync(string token, TokenType tokenType, bool includeUserLogins = false, bool includeUserTokens = false, bool includeUserRoles = false)
    {
        var query = _context.Users.AsQueryable().AsSplitQuery();

        if (includeUserLogins)
        {
            query = query.Include(u => u.UserLogins);
        }

        if (includeUserTokens)
        {
            query = query.Include(u => u.UserTokens);
        }

        if (includeUserRoles)
        {
            query = query.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RoleClaims);
        }

        return query.FirstOrDefaultAsync(u => u.UserTokens.Any(t => t.Name == tokenType.ToString() && t.Value == token));
    }

    public async Task RemoveTokenAsync(TKey userId, TokenType tokenType)
    {
        var user = _context.Users
            .Where(u => u.Id.Equals(userId))
            .Include(u => u.UserTokens)
            .FirstOrDefault();

        if (user is null)
        {
            return;
        }

        user.UserTokens ??= [];

        var existingToken = user.UserTokens.FirstOrDefault(t => t.Name == tokenType.ToString() && t.UserId.Equals(userId));

        if (existingToken is null)
        {
            return;
        }

        await RemoveUserTokenAsync(user, existingToken);
    }

    public Task RemoveTokenAsync(AppUser<TKey> user, TokenType tokenType)
    {
        var existingToken = user.UserTokens.FirstOrDefault(t => t.Name == tokenType.ToString() && t.UserId.Equals(user.Id) && t.LoginProvider == (user.UserLogins?.FirstOrDefault()?.LoginProvider ?? AppConstants.DefaultLoginProvider));

        if (existingToken is null)
        {
            return Task.CompletedTask;
        }

        return RemoveUserTokenAsync(user, existingToken);
    }
}
