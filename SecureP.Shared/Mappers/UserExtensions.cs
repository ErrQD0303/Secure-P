using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Shared.Mappers
{
    public static class UserModelExtensions
    {
        public static GetUserDto<TKey> ToGetUserDto<TKey>(this AppUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return new GetUserDto<TKey>
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed!,
                PhoneNumber = user.PhoneNumber!,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                DayOfBirth = user.DayOfBirth,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                FullName = user.FullName,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                UserTokens = [.. user.UserTokens.Select(ut => ut.ToGetUserTokenDto())],
                LicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []],
                AccessFailedCount = user.AccessFailedCount,
                PostCode = user.PostCode,
                Avatar = user.Avatar
            };
        }


        public static NewUserDto<TKey> ToNewUserDto<TKey>(this AppUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return new NewUserDto<TKey>
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                DayOfBirth = user.DayOfBirth,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                FullName = user.FullName,
                PostCode = user.PostCode,
                LicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []],
                Avatar = user.Avatar
            };
        }

        public static GetUserInfoResponseAppUser<TKey> ToGetUserInfoResponseAppUser<TKey>(this AppUser<TKey> user, List<string> roles, List<string> permissions) where TKey : IEquatable<TKey>
        {
            return new GetUserInfoResponseAppUser<TKey>
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed!,
                PhoneNumber = user.PhoneNumber!,
                DayOfBirth = user.DayOfBirth!,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                FullName = user.FullName,
                PostCode = user.PostCode,
                UserLicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []],
                Avatar = user.Avatar,
                Roles = roles,
                Permissions = permissions
            };
        }

        public static UserRegisterResponseAppUser<TKey> ToUserRegisterResponseAppUser<TKey>(this AppUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return new UserRegisterResponseAppUser<TKey>
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                EmailConfirmed = user.EmailConfirmed!,
                PhoneNumber = user.PhoneNumber!,
                DayOfBirth = user.DayOfBirth!,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                FullName = user.FullName,
                PostCode = user.PostCode,
                UserLicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []],
                Avatar = user.Avatar
            };
        }

        public static LoginResponseAppUser<TKey> ToLoginResponseAppUser<TKey>(this AppUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return user == null
                ? throw new ArgumentNullException(nameof(user))
                : new LoginResponseAppUser<TKey>
                {
                    Id = user.Id,
                    Username = user?.UserName ?? string.Empty,
                    Email = user?.Email ?? string.Empty,
                    EmailConfirmed = user!.EmailConfirmed,
                    PhoneNumber = user?.PhoneNumber ?? string.Empty,
                    FullName = user?.FullName ?? string.Empty,
                    DayOfBirth = user!.DayOfBirth,
                    Country = user.Country,
                    City = user.City,
                    AddressLine1 = user.AddressLine1,
                    AddressLine2 = user.AddressLine2,
                    PostCode = user.PostCode,
                    UserLicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []],
                    Avatar = user.Avatar
                };
        }

        public static RegisterValidatedUserDto<TKey> ToRegisterValidatedUserDto<TKey>(this RegisterRequest registerRequest) where TKey : IEquatable<TKey>
        {
            return new RegisterValidatedUserDto<TKey>
            {
                UserName = registerRequest.Username ?? registerRequest.Email,
                Password = registerRequest.Password,
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.PhoneNumber,
                FullName = registerRequest.FullName,
                DayOfBirth = registerRequest.DayOfBirth,
                Country = registerRequest.Country,
                City = registerRequest.City,
                AddressLine1 = registerRequest.AddressLine1,
                AddressLine2 = registerRequest.AddressLine2,
                PostCode = registerRequest.PostCode ?? string.Empty,
                LicensePlates = registerRequest.LicensePlates,
            };
        }

        public static AppUser<TKey> ToAppUser<TKey>(this RegisterValidatedUserDto<TKey> user, UserManager<AppUser<TKey>> userManager) where TKey : IEquatable<TKey>
        {
            var newUser = new AppUser<TKey>
            {
                UserName = user.UserName ?? user.Email,
                Email = user.Email,
                EmailConfirmed = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                TwoFactorEnabled = userManager.Options.SignIn.RequireConfirmedEmail,
                LockoutEnabled = userManager.Options.Lockout.AllowedForNewUsers,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName ?? string.Empty,
                DayOfBirth = user.DayOfBirth,
                Country = user.Country ?? string.Empty,
                City = user.City ?? string.Empty,
                AddressLine1 = user.AddressLine1 ?? string.Empty,
                AddressLine2 = user.AddressLine2,
                PostCode = user.PostCode ?? string.Empty,
                Id = typeof(TKey) switch
                {
                    Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                    Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid().ToString(),
                    _ => throw new NotImplementedException()
                },
            };

            newUser.UserLicensePlates = [.. user.LicensePlates.Select(lp => new AppUserLicensePlate<TKey>
            {
                LicensePlateNumber = lp,
                UserId = newUser.Id,
            })];

            return newUser;
        }
    }
}