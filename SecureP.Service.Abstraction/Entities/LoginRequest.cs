using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureP.Service.Abstraction.Entities;

public class LoginRequest
{
    public string Password { get; set; }
}

public class LoginByEmailRequest : LoginRequest
{
    public string Email { get; set; }
}

public class LoginByUsernameRequest : LoginRequest
{
    public string Username { get; set; }
}