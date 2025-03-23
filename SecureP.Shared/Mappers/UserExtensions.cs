using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                PostCode = user.PostCode
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
            };
        }

        public static GetUserInfoResponseAppUser<TKey> ToGetUserInfoResponseAppUser<TKey>(this AppUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return new GetUserInfoResponseAppUser<TKey>
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                DayOfBirth = user.DayOfBirth!,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                FullName = user.FullName,
                PostCode = user.PostCode,
                UserLicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []]
            };
        }

        public static UserRegisterResponseAppUser<TKey> ToUserRegisterResponseAppUser<TKey>(this AppUser<TKey> user) where TKey : IEquatable<TKey>
        {
            return new UserRegisterResponseAppUser<TKey>
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                DayOfBirth = user.DayOfBirth!,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                FullName = user.FullName,
                PostCode = user.PostCode,
                UserLicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []]
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
                    PhoneNumber = user?.PhoneNumber ?? string.Empty,
                    FullName = user?.FullName ?? string.Empty,
                    DayOfBirth = user!.DayOfBirth,
                    Country = user.Country,
                    City = user.City,
                    AddressLine1 = user.AddressLine1,
                    AddressLine2 = user.AddressLine2,
                    PostCode = user.PostCode,
                    UserLicensePlates = [.. user.UserLicensePlates?.Select(lp => lp.LicensePlateNumber) ?? []]
                };
        }
    }
}