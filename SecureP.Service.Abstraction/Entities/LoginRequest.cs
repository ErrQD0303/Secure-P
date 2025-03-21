namespace SecureP.Service.Abstraction.Entities;

public class LoginRequest
{
    public string Password { get; set; } = null!;
}

public class LoginByEmailRequest : LoginRequest
{
    public string Email { get; set; } = null!;
}

public class LoginByUsernameRequest : LoginRequest
{
    public string Username { get; set; } = null!;
}
public class LoginByPhoneNumberRequest : LoginRequest
{
    public string Phone { get; set; } = null!;
}