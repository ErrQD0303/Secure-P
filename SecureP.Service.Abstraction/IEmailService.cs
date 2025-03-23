using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SecureP.Service.Abstraction
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(string userEmail, string otp);
    }
}